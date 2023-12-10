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
    private static async Task<int> Main(string[] args)
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
                "print user manual after this message",
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
            await Console.Error.WriteLineAsync($"error: invalid argument, {exception.Message}");

            return 1;
        }
        catch (OptionException exception)
        {
            await Console.Error.WriteLineAsync($"error: invalid option, {exception.Message}");

            return 1;
        }

        // Display help on request or missing input arguments
        if (displayHelp || remainder.Count < 1)
        {
            ShowHelp(Console.Error, options, displayHelp);

            return 0;
        }

        // Read assemblies and query from input arguments, then execute query on target assemblies
        var assemblies = await LoadAssemblies(remainder.Skip(1), ".", true);
        var query = readFile ? await File.ReadAllTextAsync(remainder[0]) : remainder[0];

        if (assemblies.Count == 0)
            await Console.Error.WriteLineAsync("warning: empty assemblies list passed as argument");

        return await ExecuteQuery(assemblies, arguments, query, printer);
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

    private static async Task<int> ExecuteQuery(IEnumerable<string> assemblies, IReadOnlyList<string> arguments,
        string query, IPrinter printer)
    {
        try
        {
            await Engine.QueryAndPrint(assemblies, arguments, Engine.NormalizeQuery(query), printer);

            return 0;
        }
        catch (CompilationErrorException exception)
        {
            await Console.Error.WriteLineAsync($"error: could not compile query, {exception.Message}");

            return 2;
        }
    }

    private static async Task<IReadOnlyList<string>> LoadAssemblies(IEnumerable<string> sources, string parent,
        bool strict)
    {
        var assemblies = new List<string>();

        foreach (var source in sources)
        {
            if (Directory.Exists(source))
            {
                var childSources = Directory.EnumerateFileSystemEntries(source);
                var childAssemblies = await LoadAssemblies(childSources, source, false);

                assemblies.AddRange(childAssemblies);
            }
            else if (File.Exists(source))
            {
                var extension = Path.GetExtension(source);

                if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) ||
                    extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    assemblies.Add(source);
                }
                else if (extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    var baseDirectory = Path.GetDirectoryName(source) ?? ".";
                    var lines = await File.ReadAllLinesAsync(source);
                    var childSources = lines.Select(line => Path.Combine(baseDirectory, line));
                    var childAssemblies = await LoadAssemblies(childSources, source, true);

                    assemblies.AddRange(childAssemblies);
                }
                else if (strict)
                    await Console.Error.WriteLineAsync($"warning: ignoring unsupported file '{source}' in '{parent}'");
            }
            else if (strict)
                await Console.Error.WriteLineAsync($"warning: ignoring missing file '{source}' in '{parent}'");
        }

        return assemblies;
    }

    private static void ShowHelp(TextWriter writer, OptionSet options, bool verbose)
    {
        writer.WriteLine(".NET assembly query utility");
        writer.WriteLine();
        writer.WriteLine("Usage: NBrowse [options] Query Assembly1 [Assembly2...]");
        writer.WriteLine("Example: NBrowse \"project.Assemblies.SelectMany(a => a.Types)\" MyAssembly.dll");
        writer.WriteLine();
        writer.WriteLine("  Query can be any valid C# code processing loaded assemblies into an output");
        writer.WriteLine("  Assembly[N] can be a .{dll,exe} file, directory or .txt file with one assembly per line");

        options.WriteOptionDescriptions(writer);

        if (verbose)
            Help.Write(writer);
    }
}