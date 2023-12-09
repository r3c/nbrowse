using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NBrowse.Execution;
using NBrowse.Reflection.Mono;

namespace NBrowse;

public static class Engine
{
    private const string Symbol = "[A-Za-z_][A-Za-z0-9_]*";
    private const string White = @"(?:/\*(?:[^*]|\*[^/])*\*/|//[^\n\r]*[\n\r]|\s)*";

    private readonly record struct Normalizer(Regex Regex, Func<GroupCollection, string> Formatter);

    private static readonly IReadOnlyList<Normalizer> Normalizers = new Normalizer[]
    {
        new(
            new(
                @$"^{White}(?:\((?<p>{Symbol})\)|(?<p>{Symbol})){White}=>{White}(?<b>.*)$",
                RegexOptions.Singleline),
            groups => $"({groups["p"].Value}, _) => {groups["b"].Value}"
        ),
        new(
            new(
                @$"^{White}\((?<p>{Symbol}){White},{White}(?<a>{Symbol}){White}\){White}=>{White}(?<b>.*)$",
                RegexOptions.Singleline),
            groups => $"({groups["p"].Value}, {groups["a"].Value}) => {groups["b"].Value}"
        )
    };

    public static string NormalizeQuery(string query)
    {
        foreach (var (regex, format) in Normalizers)
        {
            var match = regex.Match(query);

            if (match.Success)
                return format(match.Groups);
        }

        return $"(project, arguments) => {query}";
    }

    public static async Task QueryAndPrint(IEnumerable<string> sources, IReadOnlyList<string> arguments,
        string query, IPrinter printer)
    {
        var evaluator = Evaluator.CreateRoslyn();

        using var project = new CecilProject(sources);

        printer.Print(await evaluator.Evaluate<object>(project, arguments, query));
    }
}