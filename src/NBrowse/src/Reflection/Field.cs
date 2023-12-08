using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
    public abstract class Field : IEquatable<Field>
    {
        public static bool operator ==(Field lhs, Field rhs)
        {
            return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
        }

        public static bool operator !=(Field lhs, Field rhs)
        {
            return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
        }

        [Description("Custom attributes")]
        [JsonIgnore]
        public abstract IEnumerable<Attribute> Attributes { get; }

        [Description("Field binding to parent type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Binding Binding { get; }

        [Description("Unique human-readable identifier")]
        public abstract string Identifier { get; }

        [Description("Field name")]
        public abstract string Name { get; }

        [Description("Parent type")]
        [JsonIgnore]
        public abstract Type Parent { get; }

        [Description("Field type")]
        public abstract Type Type { get; }

        [Description("Field visibility")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Visibility Visibility { get; }

        public abstract bool Equals(Field other);

        public override bool Equals(object obj)
        {
            return obj is Field other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Field={this.Identifier}}}";
        }
    }
}