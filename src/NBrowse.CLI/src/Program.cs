using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Mono.Options;
using NBrowse.Execution;

namespace NBrowse.CLI
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var arguments = new List<string>();
            var command = false;
            var displayHelp = false;
            var printer = Printer.CreatePretty(Console.Out);
            var sources = Array.Empty<string>();

            var options = new OptionSet
            {
                { "a|argument=", "append string to `arguments` variable", a => arguments.Add(a) },
                { "c|command", "assume first argument is a query, not a file path", s => command = s != null },
                {
                    "f|format=", "change output format (value: csv, json, pretty)",
                    f => printer = CreatePrinter(f)
                },
                { "h|help", "show this message and user manual", h => displayHelp = true },
                {
                    "i|input=", "read assemblies from text file lines (value: path)",
                    i => sources = File.ReadAllLines(i)
                }
            };

            List<string> remainder;

            try
            {
                remainder = options.Parse(args);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Console.Error.WriteLine($"error: invalid argument, {exception.Message}");
                Environment.Exit(1);

                return;
            }
            catch (OptionException exception)
            {
                Console.Error.WriteLine($"error: invalid option, {exception.Message}");
                Environment.Exit(1);

                return;
            }

            // Display help on request or missing input arguments
            if (displayHelp || remainder.Count < 1)
            {
                ShowHelp(Console.Error, options, displayHelp);

                return;
            }

            // Read assemblies and query from input arguments, then execute query on target assemblies
            var assemblies = ReadAssemblies(sources.Concat(remainder.Skip(1)).ToArray());
            var query = command ? remainder[0] : File.ReadAllText(remainder[0]);

            if (assemblies.Count == 0)
                Console.Error.WriteLine("warning: empty assemblies list passed as argument");

            ExecuteQuery(assemblies, arguments, query, printer).Wait();
        }

        private static IPrinter CreatePrinter(string format)
        {
            switch (format)
            {
                case "csv":
                    return Printer.CreateCsv(Console.Out);

                case "json":
                    return Printer.CreateJson(Console.Out);

                case "plain":
                    Console.Error.WriteLine("warning: obsolete format 'plain', please use 'pretty' instead");

                    goto case "pretty";

                case "pretty":
                    return Printer.CreatePretty(Console.Out);

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, "unknown output format");
            }
        }

        private static async Task ExecuteQuery(IEnumerable<string> assemblies, IReadOnlyList<string> arguments,
            string query, IPrinter printer)
        {
            try
            {
                await Engine.QueryAndPrint(assemblies, arguments, Engine.NormalizeQuery(query), printer);
            }
            catch (CompilationErrorException exception)
            {
                Console.Error.WriteLine($"error: could not compile query, {exception.Message}");

                Environment.Exit(3);
            }
        }

        private static IReadOnlyList<string> ReadAssemblies(IEnumerable<string> sources)
        {
            var assemblies = new List<string>();

            foreach (var source in sources)
            {
                if (Directory.Exists(source))
                {
                    assemblies.AddRange(ReadAssemblies(Directory.EnumerateDirectories(source)));
                    assemblies.AddRange(Directory.EnumerateFiles(source, "*.dll"));
                }
                else if (File.Exists(source))
                    assemblies.Add(source);
                else
                {
                    Console.Error.WriteLine($"error: '{source}' is not an assembly nor a directory");
                    Environment.Exit(2);
                }
            }

            return assemblies;
        }

        private static void ShowHelp(TextWriter writer, OptionSet options, bool verbose)
        {
            writer.WriteLine(".NET assembly query utility");
            writer.WriteLine();
            writer.WriteLine("Usage: NBrowse [options] PathOrQuery AssemblyOrDirectory1 [AssemblyOrDirectory2...]");
            writer.WriteLine("Example: NBrowse -c \"p => p.Assemblies.SelectMany(a => a.Types)\" MyAssembly.dll");
            writer.WriteLine();

            options.WriteOptionDescriptions(writer);

            if (verbose)
                Help.Write(writer);
        }
    }
}