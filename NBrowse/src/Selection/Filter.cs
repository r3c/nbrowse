using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace NBrowse.Selection
{
    class Filter
    {
        // See: https://github.com/jbevain/cecil/wiki/HOWTO
		public static readonly Func<TypeDefinition, bool> IsVisible = member => member.CustomAttributes.All(attribute => attribute.AttributeType.FullName != typeof(CompilerGeneratedAttribute).FullName);
    }
}