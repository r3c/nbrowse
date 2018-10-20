using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace NBrowse.Reflection
{
    static class Discovery
    {
        // See: https://github.com/jbevain/cecil/wiki/HOWTO
        public static readonly Func<IMemberDefinition, bool> IsVisible = member => member.CustomAttributes.All(attribute => attribute.AttributeType.FullName != typeof(CompilerGeneratedAttribute).FullName);
    }
}