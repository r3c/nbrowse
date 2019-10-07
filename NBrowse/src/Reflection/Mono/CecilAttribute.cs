using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAttribute : IAttribute
	{
		public IEnumerable<object> Arguments =>
			this.attribute.ConstructorArguments.Select(argument => argument.Value);

		public IMethod Constructor => new CecilMethod(this.attribute.Constructor, this.parent);

		public string Identifier => $"{this.Type.Identifier}({string.Join(", ", this.Arguments)})";

		public IType Type => new CecilType(this.attribute.AttributeType, this.parent);

		private readonly CustomAttribute attribute;
		private readonly IAssembly parent;

		public CecilAttribute(CustomAttribute attribute, IAssembly parent)
		{
			this.attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
			this.parent = parent;
		}

		public bool Equals(IAttribute other)
		{
			return other != null && this.Identifier == other.Identifier;
		}

		public override bool Equals(object obj)
		{
			return obj is CecilAttribute other && this.Equals(other);
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
