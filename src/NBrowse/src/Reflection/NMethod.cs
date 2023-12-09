using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection;

public abstract class NMethod : IEquatable<NMethod>
{
    public static bool operator ==(NMethod lhs, NMethod rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
    }

    public static bool operator !=(NMethod lhs, NMethod rhs)
    {
        return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
    }

    [Description("Arguments")]
    [JsonIgnore]
    public abstract IEnumerable<NArgument> Arguments { get; }

    [Description("Custom attributes")]
    [JsonIgnore]
    public abstract IEnumerable<NAttribute> Attributes { get; }

    [Description("Binding to parent type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NBinding NBinding { get; }

    [Description("Method implementation information")]
    [JsonIgnore]
    public abstract NImplementation NImplementation { get; }

    [Description("Method definition")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NDefinition NDefinition { get; }

    [Description("Unique human-readable identifier")]
    public abstract string Identifier { get; }

    [Description("Method name")] public abstract string Name { get; }

    [Description("Generic parameters")]
    [JsonIgnore]
    public abstract IEnumerable<NParameter> Parameters { get; }

    [Description("Parent type")]
    [JsonIgnore]
    public abstract NType Parent { get; }

    [Description("Return type")] public abstract NType ReturnNType { get; }

    [Description("Method visibility")]
    [JsonConverter(typeof(StringEnumConverter))]
    public abstract NVisibility NVisibility { get; }

    public abstract bool Equals(NMethod other);

    public override bool Equals(object obj)
    {
        return obj is NMethod other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{Method={Identifier}}}";
    }
}