using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public interface IImplementation
	{
		[Description("Methods referenced in code")]
		[JsonIgnore]
		IEnumerable<IMethod> ReferencedMethods { get; }

		[Description("Types referenced in code")]
		[JsonIgnore]
		IEnumerable<IType> ReferencedTypes { get; }
	}
}