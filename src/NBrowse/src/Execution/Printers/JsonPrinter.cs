using System.IO;
using Newtonsoft.Json;

namespace NBrowse.Execution.Printers
{
    internal class JsonPrinter : IPrinter
    {
        private readonly TextWriter output;

        public JsonPrinter(TextWriter output)
        {
            this.output = output;
        }

        public void Print<TValue>(TValue result)
        {
            this.output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}