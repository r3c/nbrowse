using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public interface IProject
	{
		[Description("Loaded assemblies")]
		[JsonIgnore]
		IEnumerable<IAssembly> Assemblies { get; }

		[Description("Find multiple assemblies by name")]
		IEnumerable<IAssembly> FilterAssemblies(IEnumerable<string> name);

		[Description("Find assembly by name")]
		IAssembly FindAssembly(string name);

		[Description("Find method by name")]
		IMethod FindMethod(string search);

		[Description("Find type by name")]
		IType FindType(string search);
	}
}