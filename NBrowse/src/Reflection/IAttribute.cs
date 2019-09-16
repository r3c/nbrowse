using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public interface IAttribute : IEquatable<IAttribute>
	{
		[Description("Constructor arguments")]
		IEnumerable<object> Arguments { get; }

		[Description("Attribute constructor")]
		[JsonIgnore]
		IMethod Constructor { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Attribute type")]
		IType Type { get; }
	}
}