using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NBrowse.Execution;
using NBrowse.Reflection.Mono;

namespace NBrowse
{
	public static class QueryHelper
	{
		private static readonly Regex DoubleArgumentLambda =
			new Regex(@"^\s*\((?<a>[A-Za-z_][A-Za-z0-9_]*)\s*,\s*(?<b>[A-Za-z_][A-Za-z0-9_]*)\s*\)\s*=>\s*(?<body>.*)$",
				RegexOptions.Singleline);

		private static readonly Regex SingleArgumentLambda =
			new Regex(@"^\s*(?:\((?<a>[A-Za-z_][A-Za-z0-9_]*)\)|(?<a>[A-Za-z_][A-Za-z0-9_]*))\s*=>\s*(?<body>.*)$",
				RegexOptions.Singleline);

		public static string NormalizeQuery(string query)
		{
			var match1 = QueryHelper.SingleArgumentLambda.Match(query);

			if (match1.Success)
				return $"({match1.Groups["a"].Value}, arguments) => {match1.Groups["body"].Value}";

			var match2 = QueryHelper.DoubleArgumentLambda.Match(query);

			if (match2.Success)
				return $"({match2.Groups["a"].Value}, {match2.Groups["b"].Value}) => {match2.Groups["body"].Value}";

			return $"(project, arguments) => {query}";
		}

		public static async Task QueryAndPrint(IEnumerable<string> sources, IReadOnlyList<string> arguments,
			string query, IPrinter printer)
		{
			var evaluator = Evaluator.CreateRoslyn();

			using (var project = new CecilProject(sources))
			{
				printer.Print(await evaluator.Evaluate<object>(project, arguments, query));
			}
		}
	}
}