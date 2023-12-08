using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
    public abstract class Method : IEquatable<Method>
    {
        public static bool operator ==(Method lhs, Method rhs)
        {
            return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
        }

        public static bool operator !=(Method lhs, Method rhs)
        {
            return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
        }

        [Description("Arguments")]
        [JsonIgnore]
        public abstract IEnumerable<Argument> Arguments { get; }

        [Description("Custom attributes")]
        [JsonIgnore]
        public abstract IEnumerable<Attribute> Attributes { get; }

        [Description("Binding to parent type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Binding Binding { get; }

        [Description("Method implementation information")]
        [JsonIgnore]
        public abstract Implementation Implementation { get; }

        [Description("Method definition")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Definition Definition { get; }

        [Description("Unique human-readable identifier")]
        public abstract string Identifier { get; }

        [Description("Method name")]
        public abstract string Name { get; }

        [Description("Generic parameters")]
        [JsonIgnore]
        public abstract IEnumerable<Parameter> Parameters { get; }

        [Description("Parent type")]
        [JsonIgnore]
        public abstract Type Parent { get; }

        [Description("Return type")]
        public abstract Type ReturnType { get; }

        [Description("Method visibility")]
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract Visibility Visibility { get; }

        public abstract bool Equals(Method other);

        public override bool Equals(object obj)
        {
            return obj is Method other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Method={this.Identifier}}}";
        }
    }
}