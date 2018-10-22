using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var file = string.Empty;
            var help = false;
            var inputs = Enumerable.Empty<string>();
            var output = "plain";
            var query = "project => project.Assemblies";

            var options = new OptionSet
            {
                { "f|file=", "read assemblies from text file (one path per line)", f => file = f },
                { "h|help", "show this message and exit", h => help = h != null },
                { "o|output=", "change output format (plain)", o => output = o },
                { "q|query=", "read query from command line argument", q => query = q},
                { "s|source=", "read query from text file", s => query = File.ReadAllText(s)}
            };

            try
            {
                inputs = options.Parse(args);
            }
            catch (OptionException exception)
            {
                Console.Error.WriteLine("error when parsing command line arguments: " + exception.Message);

                return;
            }

            if (help)
            {
                ShowHelp(Console.Error, options);

                return;
            }

            var printer = CreatePrinter(output);

            if (!string.IsNullOrEmpty(file))
                inputs = inputs.Concat(File.ReadAllLines(file));

            var sources = new List<string>();

            foreach (string input in inputs)
            {
                if (Directory.Exists(input))
                    sources.AddRange(Directory.EnumerateFiles(input, "*.dll"));
                else if (File.Exists(input))
                    sources.Add(input);
                else
                    throw new FileNotFoundException("could not find input assembly nor directory", input);
            }

            using (var repository = new Repository(sources))
            {
                printer.Print(Console.Out, repository.Query(query).Result);
            }
        }

        private static IPrinter CreatePrinter(string output)
        {
            switch (output)
            {
                case "plain":
                    return new PlainPrinter();

                default:
                    throw new ArgumentOutOfRangeException(nameof(output), output, "unknown output format");
            }
        }

        private static void ShowHelp(TextWriter writer, OptionSet options)
        {
            writer.WriteLine(".NET assembly query utility");
            writer.WriteLine();
            writer.WriteLine("Usage: NBrowse [options] -q \"query expression\" AssemblyOrDirectory [AssemblyOrDirectory...]");
            writer.WriteLine("Example: NBrowse -q \"project => project.Assemblies.SelectMany(assembly => assembly.Types)\" MyAssembly.dll");
            writer.WriteLine();

            options.WriteOptionDescriptions(writer);

            return;
        }
    }
}