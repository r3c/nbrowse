using System;
using System.Collections.Generic;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public struct Field : IEquatable<Field>
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public Binding Binding => _field.IsStatic ? Binding.Static : Binding.Instance;

		public string Identifier => $"{Parent.Identifier}.{Name}";

		public string Name => _field.Name;

		[JsonIgnore]
		public Type Parent => new Type(_field.DeclaringType);

		public Type Type => new Type(_field.FieldType);

		[JsonConverter(typeof(StringEnumConverter))]
		public Visibility Visibility => _field.IsPublic
			? Visibility.Public
			: (_field.IsPrivate
				? Visibility.Private
				: (_field.IsFamily
					? Visibility.Protected
					: Visibility.Internal));

		private readonly FieldDefinition _field;

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
			_field = field;
		}

		public bool Equals(Field other)
		{
			return _field.MetadataToken.RID == other._field.MetadataToken.RID;
		}

		public override bool Equals(object o)
		{
			return o is Field other && Equals(other);
		}

		public override int GetHashCode()
		{
			return _field.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Field={Identifier}}}";
		}
	}
}