using System;
using System.Collections.Generic;

namespace NBrowse.Reflection
{
	public interface IAssembly : IEquatable<IAssembly>
	{
		string FileName { get; }
		string Identifier { get; }
		string Name { get; }
		IEnumerable<string> References { get; }
		Version Version { get; }
		IEnumerable<IType> Types { get; }
	}
}