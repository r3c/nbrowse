using System.IO;
using Newtonsoft.Json;

namespace NBrowse.Formatting.Printers
{
	public class JsonPrinter : IPrinter
	{
		public void Print(TextWriter writer, object result)
		{
			writer.WriteLine(JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
		}
	}
}