using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
    public abstract class Type : IEquatable<Type>
    {
        public static bool operator ==(Type lhs, Type rhs)
        {
            return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
        }

        public static bool operator !=(Type lhs, Type rhs)
        {
            return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
        }

        [Description("Generic arguments types")]
        [JsonIgnore]
        public abstract IEnumerable<Type> Arguments { get; }

        [Description("Custom attributes (resolved type only)")]
        [JsonIgnore]
        public abstract IEnumerable<Attribute> Attributes { get; }

        [Description("Base type if any or null otherwise (resolved type only)")]
        [JsonIgnore]
        public abstract Type BaseOrNull { get; }

        [Description("Type implementation (resolved type only)")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Definition Definition { get; }

        [Description("Element type if any or null otherwise")]
        [JsonIgnore]
        public abstract Type ElementOrNull { get; }

        [Description("Declared fields (resolved type only)")]
        [JsonIgnore]
        public abstract IEnumerable<Field> Fields { get; }

        [Description("Unique human-readable identifier")]
        public abstract string Identifier { get; }

        [Description("Type interfaces (resolved type only)")]
        [JsonIgnore]
        public abstract IEnumerable<Type> Interfaces { get; }

        [Description("Declared methods (resolved type only)")]
        [JsonIgnore]
        public abstract IEnumerable<Method> Methods { get; }

        [Description("Type model (resolved type only)")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Model Model { get; }

        [Description("Type name")] public abstract string Name { get; }

        [Description("Parent namespace")] public abstract string Namespace { get; }

        [Description("Declared nested types (resolved type only)")]
        [JsonIgnore]
        public abstract IEnumerable<Type> NestedTypes { get; }

        [Description("Generic parameters")]
        [JsonIgnore]
        public abstract IEnumerable<Parameter> Parameters { get; }

        [Description("Parent assembly")]
        [JsonIgnore]
        public abstract Assembly Parent { get; }

        [Description("Type visibility (resolved type only)")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Visibility Visibility { get; }

        public abstract bool Equals(Type other);

        public override bool Equals(object obj)
        {
            return obj is Type other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Type={this.Identifier}}}";
        }
    }
}