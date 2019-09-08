using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NBrowse.Reflection
{
	public interface IAssembly : IEquatable<IAssembly>
	{
		[Description("File name on disk")]
		string FileName { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Assembly name")]
		string Name { get; }

		[Description("Referenced assembly names")]
		IEnumerable<string> References { get; }

		[Description("Assembly version")]
		Version Version { get; }

		[Description("Declared types")]
		IEnumerable<IType> Types { get; }
	}
}