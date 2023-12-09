using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection;

public abstract class NField : IEquatable<NField>
{
    public static bool operator ==(NField lhs, NField rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
    }

    public static bool operator !=(NField lhs, NField rhs)
    {
        return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
    }

    [Description("Custom attributes")]
    [JsonIgnore]
    public abstract IEnumerable<NAttribute> Attributes { get; }

    [Description("Field binding to parent type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NBinding NBinding { get; }

    [Description("Unique human-readable identifier")]
    public abstract string Identifier { get; }

    [Description("Field name")] public abstract string Name { get; }

    [Description("Parent type")]
    [JsonIgnore]
    public abstract NType Parent { get; }

    [Description("Field type")] public abstract NType NType { get; }

    [Description("Field visibility")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NVisibility NVisibility { get; }

    public abstract bool Equals(NField other);

    public override bool Equals(object obj)
    {
        return obj is NField other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{Field={Identifier}}}";
    }
}