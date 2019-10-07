using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public interface IAssembly : IEquatable<IAssembly>
	{
		[Description("Custom attributes (resolved assembly only)")]
		IEnumerable<IAttribute> Attributes { get; }

		[Description("Name of assembly culture")]
		string Culture { get; }

		[Description("File name on disk (resolved assembly only)")]
		string FileName { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Assembly name")]
		string Name { get; }

		[Description("Referenced assemblies (resolved assembly only)")]
		[JsonIgnore]
		IEnumerable<IAssembly> References { get; }

		[Description("Assembly version")]
		Version Version { get; }

		[Description("Declared types (resolved assembly only)")]
		[JsonIgnore]
		IEnumerable<IType> Types { get; }
	}
}