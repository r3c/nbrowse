using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NBrowse.Reflection
 {
	 public interface IType : IEquatable<IType>
	 {
		 [Description("Type custom attributes")]
		 IEnumerable<IAttribute> Attributes { get; }

		 [Description("Base type if any, null otherwise")]
		 IType BaseOrNull { get; }

		 [Description("Declared fields")]
		 IEnumerable<IField> Fields { get; }

		 [Description("Unique human-readable identifier")]
		 string Identifier { get; }

		 [Description("Type implementation")]
		 Implementation Implementation { get; }

		 [Description("Type interfaces")]
		 IEnumerable<IType> Interfaces { get; }

		 [Description("Declared methods")]
		 IEnumerable<IMethod> Methods { get; }

		 [Description("Type model")]
		 Model Model { get; }

		 [Description("Type name")]
		 string Name { get; }

		 [Description("Parent namespace")]
		 string Namespace { get; }

		 [Description("Declared nested types")]
		 IEnumerable<IType> NestedTypes { get; }

		 [Description("Generic parameters")]
		 IEnumerable<IParameter> Parameters { get; }

		 [Description("Parent assembly")]
		 IAssembly Parent { get; }

		 [Description("Type visibility")]
		 Visibility Visibility { get; }
	 }
 }