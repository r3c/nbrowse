using System.Collections.Generic;
using System.IO;
using CsvHelper;

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
        var options = new CsvWriterOptions
        {
            Separator = ';',
            Escape = '\\',
            NewLine = "\n",
            Quoting = CsvQuoting.WhenNeeded
        };

        using var csv = CsvWriter.Create(_output, options);

        if (result is IEnumerable<object> enumerable)
            csv.WriteRecords(enumerable);
        else
            csv.WriteRecords([result]);
    }
}