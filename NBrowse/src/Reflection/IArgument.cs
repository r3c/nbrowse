using System;
using System.ComponentModel;

namespace NBrowse.Reflection
{
	public interface IArgument : IEquatable<IArgument>
	{
		[Description("True if argument has default value")]
		bool HasDefaultValue { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Argument name")]
		string Name { get; }

		[Description("Argument type")]
		IType Type { get; }
	}
}