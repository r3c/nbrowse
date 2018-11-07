using System.Collections.Generic;
using System.Linq;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
	public static class Has
	{
		// See: https://github.com/jbevain/cecil/wiki/HOWTO
		public static System.Func<IEnumerable<Attribute>, bool> Attribute<T>() where T : System.Attribute
		{
			return attributes => attributes.Any(attribute => attribute.Identifier == typeof(T).FullName);
		}
	}
}