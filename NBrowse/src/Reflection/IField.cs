using System;
using System.ComponentModel;

namespace NBrowse.Reflection
{
	public interface IField : IEquatable<IField>
	{
		[Description("Field binding to parent type")]
		Binding Binding { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Field name")]
		string Name { get; }

		[Description("Parent type")]
		IType Parent { get; }

		[Description("Field type")]
		IType Type { get; }

		[Description("Field visibility")]
		Visibility Visibility { get; }
	}
}