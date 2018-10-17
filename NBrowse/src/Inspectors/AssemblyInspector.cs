using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NBrowse.Inspectors
{
    public static class AssemblyInspector
    {
        public static string Name(this Assembly assembly) => assembly.GetName().Name;
        public static IEnumerable<Type> Types(this Assembly assembly) => assembly.GetTypes().Where(t => !Reflection.IsCompilerGenerated(t));
        public static Version Version(this Assembly assembly) => assembly.GetName().Version;
    }
}