using System.IO;
using Newtonsoft.Json;

namespace NBrowse.Execution.Printers;

internal class JsonPrinter : IPrinter
{
    private readonly TextWriter _output;

    public JsonPrinter(TextWriter output)
    {
        _output = output;
    }

    public void Print<TValue>(TValue result)
    {
        _output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}