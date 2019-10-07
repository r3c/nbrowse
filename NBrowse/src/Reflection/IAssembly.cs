using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public interface IAssembly : IEquatable<IAssembly>
	{
		[Description("Custom attributes")]
		IEnumerable<IAttribute> Attributes { get; }

		[Description("Culture")]
		string Culture { get; }

		[Description("File name on disk")]
		string FileName { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Assembly name")]
		string Name { get; }

		[Description("Referenced assemblies")]
		[JsonIgnore]
		IEnumerable<IAssembly> References { get; }

		[Description("Assembly version")]
		Version Version { get; }

		[Description("Declared types")]
		[JsonIgnore]
		IEnumerable<IType> Types { get; }
	}
}