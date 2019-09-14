using System.Collections;
using System.IO;
using System.Linq;

namespace NBrowse.Execution.Printers
{
	internal class PrettyPrinter : IPrinter
	{
		private readonly TextWriter output;

		public PrettyPrinter(TextWriter output)
		{
			this.output = output;
		}

		public void Print<TValue>(TValue result)
		{
			if (result is IEnumerable enumerable)
			{
				foreach (var item in enumerable.Cast<object>().Select(r => r.ToString()))
					this.output.WriteLine(item);
			}
			else
				this.output.WriteLine(result.ToString());
		}
	}
}