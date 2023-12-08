using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NBrowse.Execution;
using NBrowse.Reflection.Mono;

namespace NBrowse
{
    public static class Engine
    {
        private static readonly (Regex, Func<Match, string>)[] Normalizers =
        {
            (new Regex(@"^\s*(?:\((?<a>[A-Za-z_][A-Za-z0-9_]*)\)|(?<a>[A-Za-z_][A-Za-z0-9_]*))\s*=>\s*(?<body>.*)$", RegexOptions.Singleline),
                match => $"({match.Groups["a"].Value}, arguments) => {match.Groups["body"].Value}"),
            (new Regex(@"^\s*\((?<a>[A-Za-z_][A-Za-z0-9_]*)\s*,\s*(?<b>[A-Za-z_][A-Za-z0-9_]*)\s*\)\s*=>\s*(?<body>.*)$", RegexOptions.Singleline),
                match => $"({match.Groups["a"].Value}, {match.Groups["b"].Value}) => {match.Groups["body"].Value}")
        };

        public static string NormalizeQuery(string query)
        {
            foreach (var (regex, format) in Engine.Normalizers)
            {
                var match = regex.Match(query);

                if (match.Success)
                    return format(match);
            }

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