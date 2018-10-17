using System.IO;

namespace NBrowse
{
    public interface IPrinter
    {
         void Print(TextWriter writer, object result);
    }
}