using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection;

public abstract class NAssembly : IEquatable<NAssembly>
{
    public static bool operator ==(NAssembly lhs, NAssembly rhs)
    {
        return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
    }

    public static bool operator !=(NAssembly lhs, NAssembly rhs)
    {
        return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
    }

    [Description("Custom attributes (resolved assembly only)")]
    public abstract IEnumerable<NAttribute> Attributes { get; }

    [Description("Name of assembly culture")]
    public abstract string Culture { get; }

    [Description("File name on disk (resolved assembly only)")]
    public abstract string FileName { get; }

    [Description("Unique human-readable identifier")]
    public abstract string Identifier { get; }

    [Description("Assembly name")] public abstract string Name { get; }

    [Description("Referenced assemblies (resolved assembly only)")]
    [JsonIgnore]
    public abstract IEnumerable<NAssembly> References { get; }

    [Description("Assembly version")] public abstract Version Version { get; }

    [Description("Declared types (resolved assembly only)")]
    [JsonIgnore]
    public abstract IEnumerable<NType> Types { get; }

    public abstract bool Equals(NAssembly other);

    public override bool Equals(object obj)
    {
        return obj is NAssembly other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Identifier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{{Assembly={Identifier}}}";
    }
}