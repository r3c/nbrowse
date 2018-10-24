using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Mono.Options;
using NBrowse.Formatting;
using NBrowse.Formatting.Printers;
using NBrowse.Reflection;

namespace NBrowse.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var displayHelp = false;
            var printer = new PlainPrinter() as IPrinter;
            var queryIsFile = false;
            var sources = Array.Empty<string>();

            var options = new OptionSet
            {
                { "f|format=", "change output format (plain)", format => printer = CreatePrinter(format) },
                { "h|help", "show this message and exit", h => displayHelp = h != null },
                { "i|input=", "read assemblies from text file (one path per line)", i => sources = File.ReadAllLines(i) },
                { "s|source", "assume query is a text file, not a plain query", s => queryIsFile = s != null }
            };

            List<string> remainder;

            try
            {
                remainder = options.Parse(args);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Console.Error.WriteLine($"invalid argument: {exception.Message}");
                Environment.Exit(1);

                return;
            }
            catch (OptionException exception)
            {
                Console.Error.WriteLine($"error when parsing command line arguments: {exception.Message}");
                Environment.Exit(1);

                return;
            }

            // Display help on request or missing input arguments
            if (displayHelp || remainder.Count < 1)
            {
                ShowHelp(Console.Error, options);

                return;
            }

            // Read assemblies and query from input arguments, then execute query on target assemblies
            var assemblies = ReadAssemblies(sources, remainder.Skip(1));
            var query = queryIsFile ? File.ReadAllText(remainder[0]) : remainder[0];

            ExecuteQuery(assemblies, query, printer).Wait();
        }

        private static IPrinter CreatePrinter(string format)
        {
            switch (format)
            {
                case "plain":
                    return new PlainPrinter();

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, "unknown output format");
            }
        }

        private static async Task ExecuteQuery(IEnumerable<string> assemblies, string query, IPrinter printer)
        {
            using (var repository = new Repository(assemblies))
            {
                try
                {
                    var result = await repository.Query(query);

                    printer.Print(Console.Out, result);
                }
                catch (CompilationErrorException exception)
                {
                    Console.Error.WriteLine($"could not compile query: {exception.Message}");
                    Environment.Exit(3);
                }
            }
        }

        private static IEnumerable<string> ReadAssemblies(IEnumerable<string> sources, IEnumerable<string> arguments)
        {
            var assemblies = new List<string>();

            foreach (string source in sources.Concat(arguments))
            {
                if (Directory.Exists(source))
                    assemblies.AddRange(Directory.EnumerateFiles(source, "*.dll"));
                else if (File.Exists(source))
                    assemblies.Add(source);
                else
                {
                    Console.Error.WriteLine($"could not find input assembly nor directory '{source}'");
                    Environment.Exit(2);
                }
            }

            return assemblies;
        }

        private static void ShowHelp(TextWriter writer, OptionSet options)
        {
            writer.WriteLine(".NET assembly query utility");
            writer.WriteLine();
            writer.WriteLine("Usage: NBrowse [options] \"query expression\" AssemblyOrDirectory [AssemblyOrDirectory...]");
            writer.WriteLine("Example: NBrowse \"project => project.Assemblies.SelectMany(assembly => assembly.Types)\" MyAssembly.dll");
            writer.WriteLine();

            options.WriteOptionDescriptions(writer);

            writer.WriteLine();
            writer.WriteLine("Entities available in queries are:");
            writer.WriteLine();

            var entities = new Queue<System.Type>();

            entities.Enqueue(typeof(Project));

            var uniques = new HashSet<System.Type>(entities);

            while (entities.Count > 0)
            {
                var entity = entities.Dequeue();

                writer.WriteLine($"  {entity.Name}");

                if (entity.IsEnum)
                    writer.WriteLine($"     {string.Join(" | ", Enum.GetNames(entity))}");
                else if (entity.IsClass || entity.IsValueType)
                {
                    foreach (PropertyInfo property in entity.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var propertyType = property.PropertyType;

                        while (propertyType.IsGenericType)
                            propertyType = propertyType.GetGenericArguments()[0];

                        writer.WriteLine($"    .{property.Name}: {property.PropertyType}");

                        if (propertyType.Namespace == entity.Namespace && uniques.Add(propertyType))
                            entities.Enqueue(propertyType);
                    }
                }
            }
        }
    }
}