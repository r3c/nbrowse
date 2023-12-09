using System;
using System.ComponentModel;

namespace NBrowse.Reflection;

public abstract class NArgument : IEquatable<NArgument>
{
    public static bool operator ==(NArgument lhs, NArgument rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
    }

    public static bool operator !=(NArgument lhs, NArgument rhs)
    {
        return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
    }

    [Description("Default value if any or null otherwise")]
    public abstract object DefaultValue { get; }

    [Description("True if argument has default value")]
    public abstract bool HasDefaultValue { get; }

    [Description("Unique human-readable identifier")]
    public abstract string Identifier { get; }

    [Description("By-reference passing modifier")]
    public abstract NModifier NModifier { get; }

    [Description("Argument name")] public abstract string Name { get; }

    [Description("Argument type")] public abstract NType NType { get; }

    public abstract bool Equals(NArgument other);

    public override bool Equals(object obj)
    {
        return obj is NArgument other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{Argument={Name}}}";
    }
}