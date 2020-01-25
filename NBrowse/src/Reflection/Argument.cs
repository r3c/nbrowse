using System;
using System.ComponentModel;

namespace NBrowse.Reflection
{
	public abstract class Argument : IEquatable<Argument>
	{
		public static bool operator ==(Argument lhs, Argument rhs)
		{
			return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
		}

		public static bool operator !=(Argument lhs, Argument rhs)
		{
			return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
		}

		[Description("Default value if any or null otherwise")]
		public abstract object DefaultValue { get; }

		[Description("True if argument has default value")]
		public abstract bool HasDefaultValue { get; }

		[Description("Unique human-readable identifier")]
		public abstract string Identifier { get; }

		[Description("By-reference passing modifier")]
		public abstract Modifier Modifier { get; }

		[Description("Argument name")]
		public abstract string Name { get; }

		[Description("Argument type")]
		public abstract Type Type { get; }

		public abstract bool Equals(Argument other);

		public override bool Equals(object obj)
		{
			return obj is Argument other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.Identifier.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Argument={this.Name}}}";
		}
	}
}