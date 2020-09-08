using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilField : Field
    {
        public override IEnumerable<Attribute> Attributes =>
            this.field.CustomAttributes.Select(attribute => new CecilAttribute(attribute, this.project));

        public override Binding Binding => this.field.IsStatic ? Binding.Static : Binding.Instance;

		public override string Identifier => $"{this.Parent.Identifier}.{this.Name}";

		public override string Name => this.field.Name;

		public override Type Parent => new CecilType(this.field.DeclaringType, this.project);

		public override Type Type => new CecilType(this.field.FieldType, this.project);

		public override Visibility Visibility => this.field.IsPublic
			? Visibility.Public
			: (this.field.IsPrivate
				? Visibility.Private
				: (this.field.IsFamily
					? Visibility.Protected
					: Visibility.Internal));

		private readonly FieldDefinition field;
		private readonly Project project;

		public CecilField(FieldDefinition field, Project project)
		{
			this.field = field ?? throw new ArgumentNullException(nameof(field));
			this.project = project;
		}

		public override bool Equals(Field other)
		{
			return !object.ReferenceEquals(other, null) && this.Binding == other.Binding && this.Name == other.Name &&
			       this.Parent == other.Parent && this.Type == other.Type;
		}
	}
}
