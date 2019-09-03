using System;

namespace NBrowse.Reflection
{
	public interface IAttribute : IEquatable<IAttribute>
	{
		string Identifier { get; }
		IType Type { get; }
	}
}