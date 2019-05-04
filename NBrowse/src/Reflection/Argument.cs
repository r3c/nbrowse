using System;
using Mono.Cecil;

namespace NBrowse.Reflection
{
	public struct Argument : IEquatable<Argument>
	{
		public string Identifier => $"{this.Type.Identifier} {this.Name}";

		public string Name => this.argument.Name;

		public Type Type => new Type(this.argument.ParameterType);

		private readonly ParameterDefinition argument;

		public static bool operator ==(Argument lhs, Argument rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Argument lhs, Argument rhs)
		{
			return !lhs.Equals(rhs);
		}

		public Argument(ParameterDefinition argument)
		{
			this.argument = argument;
		}

		public bool Equals(Argument other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return this.Identifier == other.Identifier;
		}

		public override bool Equals(object o)
		{
			return o is Argument other && this.Equals(other);
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