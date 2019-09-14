using System.IO;
using NBrowse.Execution.Printers;

namespace NBrowse.Execution
{
	public static class Printer
	{
		public static IPrinter CreateJson(TextWriter output)
		{
			return new JsonPrinter(output);
		}

		public static IPrinter CreatePretty(TextWriter output)
		{
			return new PrettyPrinter(output);
		}
	}
}