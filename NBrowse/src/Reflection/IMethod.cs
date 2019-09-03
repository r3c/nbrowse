using System;
using System.Collections.Generic;

namespace NBrowse.Reflection
{
	public interface IMethod : IEquatable<IMethod>
	{
		IEnumerable<IArgument> Arguments { get; }
		IEnumerable<IAttribute> Attributes { get; }
		Binding Binding { get; }
		string Identifier { get; }
		Implementation Implementation { get; }
		string Name { get; }
		IEnumerable<IParameter> Parameters { get; }
		IType Parent { get; }
		IType ReturnType { get; }
		Visibility Visibility { get; }
		bool IsUsing(IMethod method);
		bool IsUsing(IType type);
	}
}