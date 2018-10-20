using System.IO;

namespace NBrowse.Formatting
{
    public interface IPrinter
    {
         void Print(TextWriter writer, object result);
    }
}