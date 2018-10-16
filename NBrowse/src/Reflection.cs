using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NBrowse
{
    static class Reflection
    {
        public static readonly BindingFlags AllBindings = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        // See: https://stackoverflow.com/questions/31542076/behavior-of-assembly-gettypes-changed-in-visual-studio-2015
        public static readonly Func<MemberInfo, bool> IsCompilerGenerated = m => Attribute.GetCustomAttribute(
            m, typeof(CompilerGeneratedAttribute)) != null;
    }
}