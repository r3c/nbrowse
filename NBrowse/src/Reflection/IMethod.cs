using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public interface IMethod : IEquatable<IMethod>
	{
		[Description("Arguments")]
		[JsonIgnore]
		IEnumerable<IArgument> Arguments { get; }

		[Description("Custom attributes")]
		[JsonIgnore]
		IEnumerable<IAttribute> Attributes { get; }

		[Description("Binding to parent type")]
		[JsonConverter(typeof(StringEnumConverter))]
		Binding Binding { get; }

		[Description("Method definition")]
		[JsonConverter(typeof(StringEnumConverter))]
		Definition Definition { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Method name")]
		string Name { get; }

		[Description("Generic parameters")]
		[JsonIgnore]
		IEnumerable<IParameter> Parameters { get; }

		[Description("Parent type")]
		[JsonIgnore]
		IType Parent { get; }

		[Description("Return type")]
		IType ReturnType { get; }

		[Description("Method visibility")]
		[JsonConverter(typeof(StringEnumConverter))]
		Visibility Visibility { get; }

		[Description("Check if referencing given method")]
		bool IsUsing(IMethod method);

		[Description("Check if referencing given type")]
		bool IsUsing(IType type);
	}
}