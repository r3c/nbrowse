using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
    public static class Is
    {
        public static System.Func<IEnumerable<Attribute>, bool> Generated = Has.Attribute<CompilerGeneratedAttribute>();

        public static System.Func<T, bool> Not<T>(System.Func<T, bool> predicate)
        {
            return input => !predicate(input);
        }
    }
}