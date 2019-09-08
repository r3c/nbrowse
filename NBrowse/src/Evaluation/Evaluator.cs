using System.Collections.Generic;
using System.Threading.Tasks;
using NBrowse.Evaluation.Evaluators;
using NBrowse.Reflection.Mono;

namespace NBrowse.Evaluation
{
	public static class Evaluator
	{
		public static Task<object> LoadAndEvaluate(IEnumerable<string> sources, string expression)
		{
			using (var project = new CecilProject(sources))
			{
				var evaluator = new RoslynEvaluator(project);

				return evaluator.Evaluate<object>(expression);
			}
		}
	}
}