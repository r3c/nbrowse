using System.Linq;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Selection;

public static class Attribute
{
    // See: https://github.com/jbevain/cecil/wiki/HOWTO
    public static bool HasAttribute<T>(this Method method) where T : System.Attribute
    {
        return method.Attributes.Any(attribute => attribute.Type.Identifier == typeof(T).FullName);
    }

    public static bool HasAttribute<T>(this Type type) where T : System.Attribute
    {
        return type.Attributes.Any(attribute => attribute.Type.Identifier == typeof(T).FullName);
    }

    public static bool IsGenerated(this Method method)
    {
        return method.HasAttribute<CompilerGeneratedAttribute>();
    }

    public static bool IsGenerated(this Type type)
    {
        return type.HasAttribute<CompilerGeneratedAttribute>();
    }
}