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

		public override Method Constructor => new CecilMethod(this.attribute.Constructor, this.parent);

		public override string Identifier => $"{this.Type.Identifier}({string.Join(", ", this.Arguments)})";

		public override Type Type => new CecilType(this.attribute.AttributeType, this.parent);

		private readonly CustomAttribute attribute;
		private readonly Assembly parent;

		public CecilAttribute(CustomAttribute attribute, Assembly parent)
		{
			this.attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
			this.parent = parent;
		}

		public override bool Equals(Attribute other)
		{
			return !object.ReferenceEquals(other, null) && this.Identifier == other.Identifier;
		}
	}
}
