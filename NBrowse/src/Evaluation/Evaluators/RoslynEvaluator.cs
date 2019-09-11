using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Reflection;
using NBrowse.Selection;

namespace NBrowse.Evaluation.Evaluators
{
	/// <summary>
	/// Evaluator implementation based on Microsoft Roslyn scripting tools.
	/// </summary>
	internal class RoslynEvaluator : IEvaluator
	{
		private static readonly Regex LambdaStyle =
			new Regex(@"^\s*(?:\((?<name>[A-Za-z_][A-Za-z0-9_]*)\)|(?<name>[A-Za-z_][A-Za-z0-9_]*))\s*=>(?<body>.*)$");

		private readonly ScriptOptions options;
		private readonly IProject project;

		public RoslynEvaluator(IProject project)
		{
			var imports = new[]
			{
				typeof(Binding).Namespace, typeof(Has).Namespace, "System", "System.Collections.Generic", "System.Linq"
			};

			var references = new[] {this.GetType().Assembly};

			this.options = ScriptOptions.Default
				.WithImports(imports)
				.WithReferences(references);
			this.project = project;
		}

		public async Task<TResult> Evaluate<TResult>(string expression)
		{
			var match = RoslynEvaluator.LambdaStyle.Match(expression);

			var statement = match.Success
				? $"({match.Groups["name"].Value}) => {match.Groups["body"].Value}"
				: $"(project) => {expression}";

			var selector = await CSharpScript.EvaluateAsync<Func<IProject, TResult>>(statement, this.options);

			return selector(this.project);
		}
	}
}