using System.Collections.Generic;

namespace NBrowse.Reflection
{
	public interface IImplementation
	{
		IEnumerable<IMethod> ReferencedMethods { get; }

		IEnumerable<IType> ReferencedTypes { get; }
	}
}