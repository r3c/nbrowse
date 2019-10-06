using System.Linq;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
	public static class Attribute
	{
		// See: https://github.com/jbevain/cecil/wiki/HOWTO
		public static bool HasAttribute<T>(this IMethod method) where T : System.Attribute
		{
			return method.Attributes.Any(attribute => attribute.Type.Identifier == typeof(T).FullName);
		}
		
		public static bool HasAttribute<T>(this IType type) where T : System.Attribute
		{
			return type.Attributes.Any(attribute => attribute.Type.Identifier == typeof(T).FullName);
		}

		public static bool IsGenerated(this IMethod method) => method.HasAttribute<CompilerGeneratedAttribute>();

		public static bool IsGenerated(this IType type) => type.HasAttribute<CompilerGeneratedAttribute>();
	}
}