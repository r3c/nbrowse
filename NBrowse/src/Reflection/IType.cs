using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
 {
	 public interface IType : IEquatable<IType>
	 {
		 [Description("Type custom attributes")]
		 [JsonIgnore]
		 IEnumerable<IAttribute> Attributes { get; }

		 [Description("Base type if any, null otherwise")]
		 [JsonIgnore]
		 IType BaseOrNull { get; }

		 [Description("Element type if any, null otherwise")]
		 [JsonIgnore]
		 IType ElementOrNull { get; }

		 [Description("Declared fields")]
		 [JsonIgnore]
		 IEnumerable<IField> Fields { get; }

		 [Description("Unique human-readable identifier")]
		 string Identifier { get; }

		 [Description("Type implementation")]
		 [JsonConverter(typeof(StringEnumConverter))]
		 Definition Definition { get; }

		 [Description("Type interfaces")]
		 [JsonIgnore]
		 IEnumerable<IType> Interfaces { get; }

		 [Description("Declared methods")]
		 [JsonIgnore]
		 IEnumerable<IMethod> Methods { get; }

		 [Description("Type model")]
		 [JsonConverter(typeof(StringEnumConverter))]
		 Model Model { get; }

		 [Description("Type name")]
		 string Name { get; }

		 [Description("Parent namespace")]
		 string Namespace { get; }

		 [Description("Declared nested types")]
		 [JsonIgnore]
		 IEnumerable<IType> NestedTypes { get; }

		 [Description("Generic parameters")]
		 [JsonIgnore]
		 IEnumerable<IParameter> Parameters { get; }

		 [Description("Parent assembly")]
		 [JsonIgnore]
		 IAssembly Parent { get; }

		 [Description("Type visibility")]
		 [JsonConverter(typeof(StringEnumConverter))]
		 Visibility Visibility { get; }
	 }
 }