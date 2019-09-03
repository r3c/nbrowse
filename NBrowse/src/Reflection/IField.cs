using System;

namespace NBrowse.Reflection
{
	public interface IField : IEquatable<IField>
	{
		Binding Binding { get; }
		string Identifier { get; }
		string Name { get; }
		IType Parent { get; }
		IType Type { get; }
		Visibility Visibility { get; }
	}
}