using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace NBrowse.Execution.Printers;

internal class CsvPrinter : IPrinter
{
    private readonly TextWriter _output;

    public CsvPrinter(TextWriter output)
    {
        _output = output;
    }

    public void Print<TValue>(TValue result)
    {
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = Encoding.UTF8,
            Escape = '\\',
            NewLine = "\n",
            Quote = '"'
        };

        using var csv = new CsvWriter(_output, configuration);

        if (result is IEnumerable enumerable)
            csv.WriteRecords(enumerable);
        else
            csv.WriteRecords(new[] { result });
    }
}