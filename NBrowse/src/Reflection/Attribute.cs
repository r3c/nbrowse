using Mono.Cecil;

namespace NBrowse.Reflection
{
	public struct Attribute
	{
		public string Identifier => this.Type.Identifier;

		public Type Type => new Type(this.attribute.AttributeType);

		private readonly CustomAttribute attribute;

		public Attribute(CustomAttribute attribute)
		{
			this.attribute = attribute;
		}

		public override string ToString()
		{
			return $"{{Attribute={this.Identifier}}}";
		}
	}
}