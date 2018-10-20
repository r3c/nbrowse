using System.Collections;
using System.IO;
using System.Linq;

namespace NBrowse.Formatting.Printers
{
	public class PrettyPrinter : IPrinter
	{
		public void Print(TextWriter writer, object result)
		{
            if (result is IEnumerable enumerable)
            {
                foreach (string item in enumerable.Cast<object>().Select(r => r.ToString()))
                    writer.WriteLine(item);
            }
            else
                writer.WriteLine(result.ToString());
		}
	}
}