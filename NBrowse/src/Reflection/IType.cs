using System;
using System.Collections.Generic;

namespace NBrowse.Reflection
 {
	 public interface IType : IEquatable<IType>
	 {
		 IEnumerable<IAttribute> Attributes { get; }
		 IType BaseOrNull { get; }
		 IEnumerable<IField> Fields { get; }
		 string Identifier { get; }
		 Implementation Implementation { get; }
		 IEnumerable<IType> Interfaces { get; }
		 IEnumerable<IMethod> Methods { get; }
		 Model Model { get; }
		 string Name { get; }
		 string Namespace { get; }
		 IEnumerable<IType> NestedTypes { get; }
		 IEnumerable<IParameter> Parameters { get; }
		 IAssembly Parent { get; }
		 Visibility Visibility { get; }
	 }
 }