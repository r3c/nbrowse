using System;
using System.ComponentModel;

namespace NBrowse.Reflection
{
	public interface IArgument : IEquatable<IArgument>
	{
		[Description("Default value if any or null otherwise")]
		object DefaultValue { get; }

		[Description("True if argument has default value")]
		bool HasDefaultValue { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("By-reference passing modifier")]
		Modifier Modifier { get; }

		[Description("Argument name")]
		string Name { get; }

		[Description("Argument type")]
		IType Type { get; }
	}
}