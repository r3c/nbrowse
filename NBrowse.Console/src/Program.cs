using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;
using NBrowse.Model;
using NBrowse.Printers;

namespace NBrowse.Console
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
                { "f|file=", "read assemblies list from given text file (one path per line)", f => file = f },
                { "h|help", "show this message and exit", h => help = h != null },
                { "o|output=", "change output format", o => output = o },
                { "q|query=", "read query from command line argument", q => query = q},
                { "s|source=", "read query from text file", s => query = File.ReadAllText(s)}
            };

            try
            {
                sources = options.Parse(args);
            }
            catch (OptionException exception)
            {
                System.Console.Error.WriteLine("error when parsing command line arguments: " + exception.Message);

                return;
            }

            if (help)
            {
                ShowHelp(System.Console.Error, options);

                return;
            }

            var printer = CreatePrinter(output);

            if (!string.IsNullOrEmpty(file))
                sources = sources.Concat(File.ReadAllLines(file));

            var assemblies = sources.Select(path => Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, path)));
            var input = assemblies.Select(a => new AssemblyModel(a)).ToArray();
            var script = new Script(assemblies);

            printer.Print(System.Console.Out, script.Execute(input, query).Result);
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
            writer.WriteLine("NBrowse:");

            options.WriteOptionDescriptions(writer);

            return;
        }
    }
}