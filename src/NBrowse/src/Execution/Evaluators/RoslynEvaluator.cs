using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Reflection;
using NBrowse.Selection;

namespace NBrowse.Execution.Evaluators;

/// <summary>
/// Evaluator implementation based on Microsoft Roslyn scripting tools.
/// </summary>
internal class RoslynEvaluator : IEvaluator
{
    private readonly ScriptOptions _options;

    public RoslynEvaluator()
    {
        var imports = new[]
        {
            typeof(NBinding).Namespace, typeof(Usage).Namespace, "System", "System.Collections.Generic",
            "System.Linq"
        };

        var references = new[] { GetType().Assembly };

        _options = ScriptOptions.Default
            .WithImports(imports)
            .WithReferences(references);
    }

    public async Task<TResult> Evaluate<TResult>(NProject nProject, IReadOnlyList<string> arguments,
        string expression)
    {
        var selector =
            await CSharpScript.EvaluateAsync<Func<NProject, IReadOnlyList<string>, TResult>>(expression,
                _options);

        return selector(nProject, arguments);
    }
}