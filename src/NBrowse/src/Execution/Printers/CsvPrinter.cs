using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace NBrowse.Execution.Printers
{
    internal class CsvPrinter : IPrinter
    {
        private readonly TextWriter output;

        public CsvPrinter(TextWriter output)
        {
            this.output = output;
        }

        public void Print<TValue>(TValue result)
        {
            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            { Delimiter = ";", Encoding = Encoding.UTF8, Escape = '\\', NewLine = NewLine.LF, Quote = '"' };

            using (var csv = new CsvWriter(this.output, configuration))
            {
                if (result is IEnumerable enumerable)
                    csv.WriteRecords(enumerable);
                else
                    csv.WriteRecords(new[] { result });
            }
        }
    }
}