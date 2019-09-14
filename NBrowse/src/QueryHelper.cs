using System.Collections.Generic;
using System.Threading.Tasks;
using NBrowse.Execution;
using NBrowse.Reflection.Mono;

namespace NBrowse
{
	public static class QueryHelper
	{
		public static async Task QueryAndPrint(IEnumerable<string> sources, string query, IPrinter printer)
		{
			var evaluator = Evaluator.CreateRoslyn();

			using (var project = new CecilProject(sources))
			{
				printer.Print(await evaluator.Evaluate<object>(project, query));
			}
		}
	}
}