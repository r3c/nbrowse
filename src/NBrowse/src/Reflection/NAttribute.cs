using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection;

public abstract class NAttribute : IEquatable<NAttribute>
{
    public static bool operator ==(NAttribute lhs, NAttribute rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
    }

    public static bool operator !=(NAttribute lhs, NAttribute rhs)
    {
        return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
    }

    [Description("Constructor arguments")] public abstract IEnumerable<object> Arguments { get; }

    [Description("Attribute constructor")]
    [JsonIgnore]
    public abstract NMethod Constructor { get; }

    [Description("Unique human-readable identifier")]
    public abstract string Identifier { get; }

    [Description("Attribute type")] public abstract NType NType { get; }

    public abstract bool Equals(NAttribute other);

    public override bool Equals(object obj)
    {
        return obj is NAttribute other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{Attribute={Identifier}}}";
    }
}