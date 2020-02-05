using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Reflection;
using NBrowse.Selection;

namespace NBrowse.Execution.Evaluators
{
	/// <summary>
	/// Evaluator implementation based on Microsoft Roslyn scripting tools.
	/// </summary>
	internal class RoslynEvaluator : IEvaluator
	{
		private readonly ScriptOptions options;

		public RoslynEvaluator()
		{
			var imports = new[]
			{
				typeof(Binding).Namespace, typeof(Usage).Namespace, "System", "System.Collections.Generic",
				"System.Linq"
			};

			var references = new[] {this.GetType().Assembly};

			this.options = ScriptOptions.Default
				.WithImports(imports)
				.WithReferences(references);
		}

		public async Task<TResult> Evaluate<TResult>(Project project, IReadOnlyList<string> arguments,
			string expression)
		{
			var selector =
				await CSharpScript.EvaluateAsync<Func<Project, IReadOnlyList<string>, TResult>>(expression,
					this.options);

			return selector(project, arguments);
		}
	}
}