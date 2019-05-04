using System.Linq;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
	public static class Has
	{
		// See: https://github.com/jbevain/cecil/wiki/HOWTO
		public static bool Attribute<T>(Type type) where T : System.Attribute
		{
			return type.Attributes.Any(attribute => attribute.Identifier == typeof(T).FullName);
		}
	}
}