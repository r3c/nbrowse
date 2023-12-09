using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection;

public abstract class Parameter : IEquatable<Parameter>
{
    public static bool operator ==(Parameter lhs, Parameter rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
    }

    public static bool operator !=(Parameter lhs, Parameter rhs)
    {
        return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
    }

    [Description("Type constraints")] public abstract IEnumerable<Type> Constraints { get; }

    [Description("True if has default constructor constraint")]
    public abstract bool HasDefaultConstructorConstraint { get; }

    [Description("True if has reference type constraint")]
    public abstract bool HasReferenceTypeConstraint { get; }

    [Description("True if has (non-nullable) value type constraint")]
    public abstract bool HasValueTypeConstraint { get; }

    [Description("Unique human-readable identifier")]
    public abstract string Identifier { get; }

    [Description("Parameter name")] public abstract string Name { get; }

    [Description("Parameter variance")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract Variance Variance { get; }

    public abstract bool Equals(Parameter other);

    public override bool Equals(object obj)
    {
        return obj is Parameter other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{Parameter={Identifier}}}";
    }
}