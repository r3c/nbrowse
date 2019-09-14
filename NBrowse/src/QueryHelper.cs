using System.Collections.Generic;
using System.Threading.Tasks;
using NBrowse.Execution;
using NBrowse.Reflection.Mono;

namespace NBrowse
{
	public static class QueryHelper
	{
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