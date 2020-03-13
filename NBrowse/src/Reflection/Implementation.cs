using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public abstract class Implementation
	{
		[Description("Implementation parent method")]
		[JsonIgnore]
		public abstract Method Parent { get; }

		[Description("Methods referenced in code")]
		[JsonIgnore]
		public abstract IEnumerable<Method> ReferencedMethods { get; }

		[Description("Types referenced in code")]
		[JsonIgnore]
		public abstract IEnumerable<Type> ReferencedTypes { get; }
	}
}