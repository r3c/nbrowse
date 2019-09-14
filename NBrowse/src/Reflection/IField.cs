using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public interface IField : IEquatable<IField>
	{
		[Description("Field binding to parent type")]
		[JsonConverter(typeof(StringEnumConverter))]
		Binding Binding { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Field name")]
		string Name { get; }

		[Description("Parent type")]
		[JsonIgnore]
		IType Parent { get; }

		[Description("Field type")]
		IType Type { get; }

		[Description("Field visibility")]
		[JsonConverter(typeof(StringEnumConverter))]
		Visibility Visibility { get; }
	}
}