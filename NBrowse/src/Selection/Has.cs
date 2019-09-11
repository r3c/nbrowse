using System.Linq;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
	public static class Has
	{
		// See: https://github.com/jbevain/cecil/wiki/HOWTO
		public static bool Attribute<T>(IMethod method) where T : System.Attribute
		{
			return method.Attributes.Any(attribute => attribute.Type.Identifier == typeof(T).FullName);
		}
		
		public static bool Attribute<T>(IType type) where T : System.Attribute
		{
			return type.Attributes.Any(attribute => attribute.Type.Identifier == typeof(T).FullName);
		}
	}
}