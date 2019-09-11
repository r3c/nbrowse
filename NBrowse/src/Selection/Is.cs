using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
    public static class Is
    {
        public static bool Generated(IMethod method) => Has.Attribute<CompilerGeneratedAttribute>(method);

        public static bool Generated(IType type) => Has.Attribute<CompilerGeneratedAttribute>(type);
    }
}