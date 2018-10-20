using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NBrowse.Reflection
{
    static class Discovery
    {
        public static readonly BindingFlags Bindings = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        // See: https://stackoverflow.com/questions/31542076/behavior-of-assembly-gettypes-changed-in-visual-studio-2015
        public static readonly Func<MemberInfo, bool> IsVisible = m => Attribute.GetCustomAttribute(
            m, typeof(CompilerGeneratedAttribute)) == null;
    }
}