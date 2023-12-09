using System.Linq;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Selection;

public static class Attribute
{
    // See: https://github.com/jbevain/cecil/wiki/HOWTO
    public static bool HasAttribute<T>(this NMethod method) where T : System.Attribute
    {
        return method.Attributes.Any(attribute => attribute.NType.Identifier == typeof(T).FullName);
    }

    public static bool HasAttribute<T>(this NType type) where T : System.Attribute
    {
        return type.Attributes.Any(attribute => attribute.NType.Identifier == typeof(T).FullName);
    }

    public static bool IsGenerated(this NMethod method)
    {
        return method.HasAttribute<CompilerGeneratedAttribute>();
    }

    public static bool IsGenerated(this NType type)
    {
        return type.HasAttribute<CompilerGeneratedAttribute>();
    }
}