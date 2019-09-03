using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAttribute : IAttribute
	{
		public string Identifier => this.Type.Identifier;

		public IType Type => new CecilType(this.attribute.AttributeType);

		private readonly CustomAttribute attribute;

		public CecilAttribute(CustomAttribute attribute)
		{
			this.attribute = attribute;
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
			return (this.attribute != null ? this.attribute.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return $"{{Attribute={this.Identifier}}}";
		}
	}
}