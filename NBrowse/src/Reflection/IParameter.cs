using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public interface IParameter
	{
		[Description("Type constraints")]
		IEnumerable<IType> Constraints { get; }

		[Description("True if has default constructor constraint")]
		bool HasDefaultConstructor { get; }

		[Description("Unique human-readable identifier")]
		string Identifier { get; }

		[Description("Parameter name")]
		string Name { get; }

		[Description("Parameter variance")]
		[JsonConverter(typeof(StringEnumConverter))]
		Variance Variance { get; }
	}
}