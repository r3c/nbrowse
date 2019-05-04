using System;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public struct Field : IEquatable<Field>
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public Binding Binding => this.field.IsStatic ? Binding.Static : Binding.Instance;

		public string Identifier => $"{this.Parent.Identifier}.{this.Name}";

		public string Name => this.field.Name;

		[JsonIgnore]
		public Type Parent => new Type(this.field.DeclaringType);

		public Type Type => new Type(this.field.FieldType);

		[JsonConverter(typeof(StringEnumConverter))]
		public Visibility Visibility => this.field.IsPublic
			? Visibility.Public
			: (this.field.IsPrivate
				? Visibility.Private
				: (this.field.IsFamily
					? Visibility.Protected
					: Visibility.Internal));

		private readonly FieldDefinition field;

		public static bool operator ==(Field lhs, Field rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Field lhs, Field rhs)
		{
			return !lhs.Equals(rhs);
		}

		public Field(FieldDefinition field)
		{
			this.field = field;
		}

		public bool Equals(Field other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return this.Identifier == other.Identifier;
		}

		public override bool Equals(object o)
		{
			return o is Field other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.field.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Field={this.Identifier}}}";
		}
	}
}