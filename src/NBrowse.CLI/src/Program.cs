using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Mono.Options;
using NBrowse.Execution;

namespace NBrowse.CLI;

internal static class Program
{
    private static void Main(string[] args)
    {
        var arguments = new List<string>();
        var displayHelp = false;
        var printer = Printer.CreatePretty(Console.Out);
        var readFile = false;

        var options = new OptionSet
        {
            {
                "a|argument=",
                "append value to `arguments` variable",
                value => arguments.Add(value)
            },
            {
                "f|file",
                "read query from file, not inline script",
                _ => readFile = true
            },
            {
                "h|help",
                "show this message and user manual",
                _ => displayHelp = true
            },
            {
                "o|output=",
                "specify output format (value: csv, json, pretty)",
                value => printer = CreatePrinter(value)
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
        var assemblies = ReadAssemblies(remainder.Skip(1)).Result;
        var query = readFile ? File.ReadAllText(remainder[0]) : remainder[0];

        if (assemblies.Count == 0)
            Console.Error.WriteLine("warning: empty assemblies list passed as argument");

        ExecuteQuery(assemblies, arguments, query, printer).Wait();
    }

    private static IPrinter CreatePrinter(string output)
    {
        switch (output)
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
                throw new ArgumentOutOfRangeException(nameof(output), output, "unknown output format");
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
            await Console.Error.WriteLineAsync($"error: could not compile query, {exception.Message}");

            Environment.Exit(2);
        }
    }

    private static async Task<IReadOnlyList<string>> ReadAssemblies(IEnumerable<string> assemblyPaths)
    {
        var assemblies = new List<string>();

        foreach (var path in assemblyPaths)
        {
            if (Directory.Exists(path))
            {
                var childAssemblies = await ReadAssemblies(Directory.EnumerateDirectories(path));

                assemblies.AddRange(childAssemblies);
                assemblies.AddRange(Directory.EnumerateFiles(path, "*.dll"));
            }
            else if (File.Exists(path))
                assemblies.Add(path);
            else
            {
                await Console.Error.WriteLineAsync(
                    $"warning: '{path}' is not a valid path to an assembly nor a directory");
            }
        }

        return assemblies;
    }

    private static void ShowHelp(TextWriter writer, OptionSet options, bool verbose)
    {
        writer.WriteLine(".NET assembly query utility");
        writer.WriteLine();
        writer.WriteLine("Usage: NBrowse [options] PathOrQuery AssemblyOrDirectory1 [AssemblyOrDirectory2...]");
        writer.WriteLine("Example: NBrowse \"project.Assemblies.SelectMany(a => a.Types)\" MyAssembly.dll");
        writer.WriteLine();

        options.WriteOptionDescriptions(writer);

        if (verbose)
            Help.Write(writer);
    }
}