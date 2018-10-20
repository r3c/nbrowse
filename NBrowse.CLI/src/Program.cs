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
            var output = "pretty";
            var query = "1";
            var sources = Enumerable.Empty<string>();

            var options = new OptionSet
            {
                { "f|file=", "read assemblies from text file (one path per line)", f => file = f },
                { "h|help", "show this message and exit", h => help = h != null },
                { "o|output=", "change output format (pretty)", o => output = o },
                { "q|query=", "read query from command line argument", q => query = q},
                { "s|source=", "read query from text file", s => query = File.ReadAllText(s)}
            };

            try
            {
                sources = options.Parse(args);
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
                sources = sources.Concat(File.ReadAllLines(file));

            using (var repository = new Repository(sources))
            {
                printer.Print(Console.Out, repository.Query(query).Result);
            }
        }

        private static IPrinter CreatePrinter(string output)
        {
            switch (output)
            {
                case "pretty":
                    return new PrettyPrinter();

                default:
                    throw new ArgumentOutOfRangeException(nameof(output), output, "unknown output format");
            }
        }

        private static void ShowHelp(TextWriter writer, OptionSet options)
        {
            writer.WriteLine(".NET assembly query utility");
            writer.WriteLine();
            writer.WriteLine("Usage: NBrowse.exe [options] -q \"query expression\" Assembly1 [Assembly2...]");
            writer.WriteLine("Example: NBrowse.exe -q \"assemblies => assemblies.SelectMany(a => a.Types())\" MyAssembly.dll");
            writer.WriteLine();

            options.WriteOptionDescriptions(writer);

            return;
        }
    }
}