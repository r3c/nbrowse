using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAttribute : Attribute
	{
		public override IEnumerable<object> Arguments =>
			this.attribute.ConstructorArguments.Select(argument => argument.Value);

		public override Method Constructor => new CecilMethod(this.attribute.Constructor, this.project);

		public override string Identifier => $"{this.Type.Identifier}({string.Join(", ", this.Arguments)})";

		public override Type Type => new CecilType(this.attribute.AttributeType, this.project);

		private readonly CustomAttribute attribute;
		private readonly Project project;

		public CecilAttribute(CustomAttribute attribute, Project project)
		{
			this.attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
			this.project = project;
		}

		public override bool Equals(Attribute other)
		{
			return !object.ReferenceEquals(other, null) && this.Constructor == other.Constructor &&
			       this.Type == other.Type && this.Arguments.SequenceEqual(other.Arguments);
		}
	}
}
