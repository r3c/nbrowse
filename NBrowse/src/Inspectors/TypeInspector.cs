using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NBrowse.Inspectors
{
    public static class TypeInspector
    {
        public static IEnumerable<FieldInfo> Fields(this Type type) => type.GetFields(Reflection.AllBindings).Where(f => !Reflection.IsCompilerGenerated(f));
    }
}