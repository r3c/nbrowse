using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection;

public abstract class NType : IEquatable<NType>
{
    public static bool operator ==(NType lhs, NType rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
    }

    public static bool operator !=(NType lhs, NType rhs)
    {
        return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
    }

    [Description("Generic arguments types")]
    [JsonIgnore]
    public abstract IEnumerable<NType> Arguments { get; }

    [Description("Custom attributes (resolved type only)")]
    [JsonIgnore]
    public abstract IEnumerable<NAttribute> Attributes { get; }

    [Description("Base type if any or null otherwise (resolved type only)")]
    [JsonIgnore]
    public abstract NType BaseOrNull { get; }

    [Description("Type implementation (resolved type only)")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NDefinition NDefinition { get; }

    [Description("Element type if any or null otherwise")]
    [JsonIgnore]
    public abstract NType ElementOrNull { get; }

    [Description("Declared fields (resolved type only)")]
    [JsonIgnore]
    public abstract IEnumerable<NField> Fields { get; }

    [Description("Unique human-readable identifier")]
    public abstract string Identifier { get; }

    [Description("Type interfaces (resolved type only)")]
    [JsonIgnore]
    public abstract IEnumerable<NType> Interfaces { get; }

    [Description("Declared methods (resolved type only)")]
    [JsonIgnore]
    public abstract IEnumerable<NMethod> Methods { get; }

    [Description("Type model (resolved type only)")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NModel NModel { get; }

    [Description("Type name")] public abstract string Name { get; }

    [Description("Parent namespace")] public abstract string Namespace { get; }

    [Description("Declared nested types (resolved type only)")]
    [JsonIgnore]
    public abstract IEnumerable<NType> NestedTypes { get; }

    [Description("Generic parameters")]
    [JsonIgnore]
    public abstract IEnumerable<NParameter> Parameters { get; }

    [Description("Parent assembly")]
    [JsonIgnore]
    public abstract NAssembly Parent { get; }

    [Description("Type visibility (resolved type only)")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NVisibility NVisibility { get; }

    public abstract bool Equals(NType other);

    public override bool Equals(object obj)
    {
        return obj is NType other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{Type={Identifier}}}";
    }
}