using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilArgument : IArgument
	{
		public bool HasDefaultValue => this.argument.HasDefault;

		public string Identifier => $"{this.Type.Identifier} {this.Name}";

		public string Name => this.argument.Name;

		public IType Type => new CecilType(this.argument.ParameterType);

		private readonly ParameterDefinition argument;

		public CecilArgument(ParameterDefinition argument)
		{
			this.argument = argument;
		}

		public bool Equals(IArgument other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return other != null && this.Identifier == other.Identifier;
		}

		public override bool Equals(object obj)
		{
			return obj is CecilArgument other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.argument.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Argument={this.Name}}}";
		}
	}
}