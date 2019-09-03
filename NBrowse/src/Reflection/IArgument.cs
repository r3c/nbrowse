using System;

namespace NBrowse.Reflection
{
	public interface IArgument : IEquatable<IArgument>
	{
		string Identifier { get; }
		string Name { get; }
		IType Type { get; }
	}
}