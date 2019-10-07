using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
 {
	 public interface IType : IEquatable<IType>
	 {
		 [Description("Custom attributes (resolved type only)")]
		 [JsonIgnore]
		 IEnumerable<IAttribute> Attributes { get; }

		 [Description("Base type if any or null otherwise (resolved type only)")]
		 [JsonIgnore]
		 IType BaseOrNull { get; }

		 [Description("Type implementation (resolved type only)")]
		 [JsonConverter(typeof(StringEnumConverter))]
		 Definition Definition { get; }

		 [Description("Element type if any or null otherwise")]
		 [JsonIgnore]
		 IType ElementOrNull { get; }

		 [Description("Declared fields (resolved type only)")]
		 [JsonIgnore]
		 IEnumerable<IField> Fields { get; }

		 [Description("Unique human-readable identifier")]
		 string Identifier { get; }

		 [Description("Type interfaces (resolved type only)")]
		 [JsonIgnore]
		 IEnumerable<IType> Interfaces { get; }

		 [Description("Declared methods (resolved type only)")]
		 [JsonIgnore]
		 IEnumerable<IMethod> Methods { get; }

		 [Description("Type model (resolved type only)")]
		 [JsonConverter(typeof(StringEnumConverter))]
		 Model Model { get; }

		 [Description("Type name")]
		 string Name { get; }

		 [Description("Parent namespace")]
		 string Namespace { get; }

		 [Description("Declared nested types (resolved type only)")]
		 [JsonIgnore]
		 IEnumerable<IType> NestedTypes { get; }

		 [Description("Generic parameters")]
		 [JsonIgnore]
		 IEnumerable<IParameter> Parameters { get; }

		 [Description("Parent assembly")]
		 [JsonIgnore]
		 IAssembly Parent { get; }

		 [Description("Type visibility (resolved type only)")]
		 [JsonConverter(typeof(StringEnumConverter))]
		 Visibility Visibility { get; }
	 }
 }