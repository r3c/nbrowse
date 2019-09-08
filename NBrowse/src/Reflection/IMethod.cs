using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NBrowse.Reflection
{
	public interface IMethod : IEquatable<IMethod>
	{
		[Description("Arguments")]
		IEnumerable<IArgument> Arguments { get; }

		[Description("Custom attributes")]
		IEnumerable<IAttribute> Attributes { get; }

		[Description("Binding to parent type")]
		Binding Binding { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Method implementation")]
		Implementation Implementation { get; }

		[Description("Method name")]
		string Name { get; }

		[Description("Generic parameters")]
		IEnumerable<IParameter> Parameters { get; }

		[Description("Parent type")]
		IType Parent { get; }

		[Description("Return type")]
		IType ReturnType { get; }

		[Description("Method visibility")]
		Visibility Visibility { get; }

		[Description("Check if referencing given method")]
		bool IsUsing(IMethod method);

		[Description("Check if referencing given type")]
		bool IsUsing(IType type);
	}
}