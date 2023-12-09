using System.Linq;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Selection;

public static class NAttribute
{
    // See: https://github.com/jbevain/cecil/wiki/HOWTO
    public static bool HasAttribute<T>(this NMethod nMethod) where T : System.Attribute
    {
        return nMethod.Attributes.Any(attribute => attribute.NType.Identifier == typeof(T).FullName);
    }

    public static bool HasAttribute<T>(this NType nType) where T : System.Attribute
    {
        return nType.Attributes.Any(attribute => attribute.NType.Identifier == typeof(T).FullName);
    }

    public static bool IsGenerated(this NMethod nMethod)
    {
        return nMethod.HasAttribute<CompilerGeneratedAttribute>();
    }

    public static bool IsGenerated(this NType nType)
    {
        return nType.HasAttribute<CompilerGeneratedAttribute>();
    }
}