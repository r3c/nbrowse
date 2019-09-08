using System;
using System.ComponentModel;

namespace NBrowse.Reflection
{
	public interface IAttribute : IEquatable<IAttribute>
	{
		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Attribute type")]
		IType Type { get; }
	}
}