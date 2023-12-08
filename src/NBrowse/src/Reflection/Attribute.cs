using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
    public abstract class Attribute : IEquatable<Attribute>
    {
        public static bool operator ==(Attribute lhs, Attribute rhs)
        {
            return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
        }

        public static bool operator !=(Attribute lhs, Attribute rhs)
        {
            return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
        }

        [Description("Constructor arguments")]
        public abstract IEnumerable<object> Arguments { get; }

        [Description("Attribute constructor")]
        [JsonIgnore]
        public abstract Method Constructor { get; }

        [Description("Unique human-readable identifier")]
        public abstract string Identifier { get; }

        [Description("Attribute type")]
        public abstract Type Type { get; }

        public abstract bool Equals(Attribute other);

        public override bool Equals(object obj)
        {
            return obj is Attribute other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Attribute={this.Identifier}}}";
        }
    }
}