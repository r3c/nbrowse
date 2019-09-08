using System.Collections.Generic;

namespace NBrowse.Reflection
{
	public interface IParameter
	{
		IEnumerable<IType> Constraints { get; }
		bool HasDefaultConstructor { get; }
		string Identifier { get; }
		string Name { get; }
		Variance Variance { get; }
		string ToString();
	}
}