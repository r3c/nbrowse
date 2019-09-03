using System.Collections.Generic;

namespace NBrowse.Reflection
{
	public interface IProject
	{
		IEnumerable<IAssembly> Assemblies { get; }
		IEnumerable<IAssembly> FilterAssemblies(IEnumerable<string> fullNames);
		IAssembly FindAssembly(string fullName);
		IMethod FindMethod(string search);
		IType FindType(string search);
	}
}