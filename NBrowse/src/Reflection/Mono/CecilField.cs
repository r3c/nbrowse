using System;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilField : IField
	{
		public Binding Binding => this.field.IsStatic ? Binding.Static : Binding.Instance;

		public string Identifier => $"{this.Parent.Identifier}.{this.Name}";

		public string Name => this.field.Name;

		public IType Parent => new CecilType(this.field.DeclaringType);

		public IType Type => new CecilType(this.field.FieldType);

		public Visibility Visibility => this.field.IsPublic
			? Visibility.Public
			: (this.field.IsPrivate
				? Visibility.Private
				: (this.field.IsFamily
					? Visibility.Protected
					: Visibility.Internal));

		private readonly FieldDefinition field;

		public CecilField(FieldDefinition field)
		{
			this.field = field ?? throw new ArgumentNullException(nameof(field));
		}

		public bool Equals(IField other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return other != null && this.Identifier == other.Identifier;
		}

		public override bool Equals(object obj)
		{
			return obj is CecilField other && this.Equals(other);
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