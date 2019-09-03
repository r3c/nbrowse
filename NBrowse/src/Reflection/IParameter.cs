using System.Collections.Generic;

namespace NBrowse.Reflection
{
	public interface IParameter
	{
		IEnumerable<IType> Constraints { get; }
		bool HasDefaultConstructor { get; }
		string Identifier { get; }
		bool IsContravariant { get; }
		bool IsCovariant { get; }
		string Name { get; }
		string ToString();
	}
}