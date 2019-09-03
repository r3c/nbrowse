using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
    public static class Is
    {
        public static System.Func<IType, bool> Generated => Has.Attribute<CompilerGeneratedAttribute>;
    }
}