using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Mono.Options;
using NBrowse.Evaluation;
using NBrowse.Formatting;
using NBrowse.Formatting.Printers;

namespace NBrowse.CLI
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var displayHelp = false;
			var printer = new PlainPrinter() as IPrinter;
			var queryIsFile = false;
			var sources = Array.Empty<string>();

			var options = new OptionSet
			{
				{ "f|format=", "change output format (value: json, plain)", format => printer = Program.CreatePrinter(format) },
				{ "h|help", "show this message and user manual", h => displayHelp = true },
				{ "i|input=", "read assemblies from text file lines (value: path)", i => sources = File.ReadAllLines(i) },
				{ "s|source", "assume query is a text file, not a plain query", s => queryIsFile = s != null }
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
				Program.ShowHelp(Console.Error, options, displayHelp);

				return;
			}

			// Read assemblies and query from input arguments, then execute query on target assemblies
			var assemblies = Program.ReadAssemblies(sources.Concat(remainder.Skip(1)).ToArray());
			var query = queryIsFile ? File.ReadAllText(remainder[0]) : remainder[0];

			if (assemblies.Count == 0)
				Console.Error.WriteLine($"warning: empty assemblies list passed as argument");

			Program.ExecuteQuery(assemblies, query, printer).Wait();
		}

		private static IPrinter CreatePrinter(string format)
		{
			switch (format)
			{
				case "json":
					return new JsonPrinter();

				case "plain":
					return new PlainPrinter();

				default:
					throw new ArgumentOutOfRangeException(nameof(format), format, "unknown output format");
			}
		}

		private static async Task ExecuteQuery(IEnumerable<string> assemblies, string query, IPrinter printer)
		{
			try
			{
				var result = await Evaluator.LoadAndEvaluate(assemblies, query);

				printer.Print(Console.Out, result);
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
					assemblies.AddRange(Program.ReadAssemblies(Directory.EnumerateDirectories(source)));
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
			writer.WriteLine("Usage: NBrowse [options] \"query expression\" AssemblyOrDirectory [AssemblyOrDirectory...]");
			writer.WriteLine("Example: NBrowse \"project => project.Assemblies.SelectMany(a => a.Types)\" MyAssembly.dll");
			writer.WriteLine();

			options.WriteOptionDescriptions(writer);

			if (verbose)
				Help.Write(writer);
		}
	}
}