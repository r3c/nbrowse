using System;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilField : Field
	{
		public override Binding Binding => this.field.IsStatic ? Binding.Static : Binding.Instance;

		public override string Identifier => $"{this.Parent.Identifier}.{this.Name}";

		public override string Name => this.field.Name;

		public override Type Parent => new CecilType(this.field.DeclaringType, this.parent);

		public override Type Type => new CecilType(this.field.FieldType, this.parent);

		public override Visibility Visibility => this.field.IsPublic
			? Visibility.Public
			: (this.field.IsPrivate
				? Visibility.Private
				: (this.field.IsFamily
					? Visibility.Protected
					: Visibility.Internal));

		private readonly FieldDefinition field;
		private readonly Assembly parent;

		public CecilField(FieldDefinition field, Assembly parent)
		{
			this.field = field ?? throw new ArgumentNullException(nameof(field));
			this.parent = parent;
		}

		public override bool Equals(Field other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return !object.ReferenceEquals(other, null) && this.Identifier == other.Identifier;
		}
	}
}
