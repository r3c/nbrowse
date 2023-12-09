using System.Collections;
using System.IO;
using System.Linq;

namespace NBrowse.Execution.Printers;

internal class PrettyPrinter : IPrinter
{
    private readonly TextWriter _output;

    public PrettyPrinter(TextWriter output)
    {
        _output = output;
    }

    public void Print<TValue>(TValue result)
    {
        if (result is IEnumerable enumerable)
            foreach (var item in enumerable.Cast<object>().Select(r => r.ToString()))
                _output.WriteLine(item);
        else
            _output.WriteLine(result.ToString());
    }
}