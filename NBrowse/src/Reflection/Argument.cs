using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace NBrowse.Reflection
{
	public struct Argument : IEquatable<Argument>
	{
		public string Identifier => $"{Type.Identifier} {Name}";

		public string Name => _argument.Name;

		public Type Type => new Type(_argument.ParameterType);

		private readonly ParameterDefinition _argument;

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
			_argument = argument;
		}

		public bool Equals(Argument other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return Identifier == other.Identifier;
		}

		public override bool Equals(object o)
		{
			return o is Argument other && Equals(other);
		}

		public override int GetHashCode()
		{
			return _argument.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Argument={Name}}}";
		}
	}
}