using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilType : IType
	{
		public IEnumerable<IAttribute> Attributes =>
			this.definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute)) ??
			Array.Empty<CecilAttribute>();

		public IType BaseOrNull => this.definition?.BaseType != null
			? new CecilType(this.definition.BaseType)
			: default(IType);

		public IType ElementOrNull => this.reference is ArrayType arrayType
			? (arrayType.ElementType != null ? new CecilType(arrayType.ElementType) : default(IType))
			: (this.reference is PointerType pointerType
				? (pointerType.ElementType != null ? new CecilType(pointerType.ElementType) : default(IType))
				: (this.reference is ByReferenceType byReferenceType
					? (byReferenceType.ElementType != null
						? new CecilType(byReferenceType.ElementType)
						: default(IType))
					: default(IType)));

		public IEnumerable<IField> Fields =>
			this.definition?.Fields.Select(field => new CecilField(field)) ?? Array.Empty<CecilField>();

		public string Identifier => $"{this.Namespace}{(string.IsNullOrEmpty(this.Namespace) ? "" : ".")}{this.Name}";

		public Definition Definition => this.definition == null
			? Definition.Unknown
			: (this.definition.IsAbstract
				? Definition.Abstract
				: (this.definition.IsSealed
					? Definition.Final
					: Definition.Virtual));

		public IEnumerable<IType> Interfaces =>
			this.definition?.Interfaces.Select(i => new CecilType(i.InterfaceType)) ?? Array.Empty<CecilType>();

		public IEnumerable<IMethod> Methods =>
			this.definition?.Methods.Select(method => new CecilMethod(method)) ?? Array.Empty<CecilMethod>();

		public Model Model => this.reference.IsValueType
			? (this.definition?.IsEnum ?? false ? Model.Enumeration : Model.Structure)
			: (this.definition?.IsInterface ?? false
				? Model.Interface
				: (this.reference.IsArray
					? Model.Array
					: (this.reference.IsPointer
						? Model.Pointer
						: (this.reference.IsByReference ? Model.Reference : Model.Class))));

		public string Name => this.reference.IsNested
			? $"{new CecilType(this.reference.DeclaringType).Name}+{this.reference.Name}"
			: this.reference.Name;

		public string Namespace => this.reference.IsNested
			? new CecilType(this.reference.DeclaringType).Namespace
			: this.reference.Namespace;

		public IEnumerable<IType> NestedTypes =>
			this.definition?.NestedTypes.Select(type => new CecilType(type)) ?? Array.Empty<CecilType>();

		public IEnumerable<IParameter> Parameters =>
			this.definition?.GenericParameters.Select(parameter => new CecilParameter(parameter)) ??
			Array.Empty<CecilParameter>();

		public IAssembly Parent => new CecilAssembly(this.reference.Module);

		public Visibility Visibility => this.definition == null
			? Visibility.Unknown
			: (this.definition.IsNested
				? (this.definition.IsNestedPublic
					? Visibility.Public
					: (this.definition.IsNestedFamily
						? Visibility.Protected
						: (this.definition.IsNestedPrivate
							? Visibility.Private
							: Visibility.Internal)))
				: (this.definition.IsPublic
					? Visibility.Public
					: (this.definition.IsNotPublic
						? Visibility.Private
						: Visibility.Internal)));

		private readonly TypeDefinition definition;
		private readonly TypeReference reference;

		public CecilType(TypeReference reference)
		{
			if (!(reference is TypeDefinition definition))
			{
				try
				{
					definition = reference.IsDefinition || reference.Module.AssemblyResolver != null
						? reference.Resolve()
						: null;
				}
				// FIXME: Mono.Cecil throws an exception when trying to resolve a
				// non-loaded assembly and I don't know how I can safely avoid that
				// without catching the exception.
				catch (AssemblyResolutionException)
				{
					definition = null;
				}
			}

			this.definition = definition;
			this.reference = reference;
		}

		public bool Equals(IType other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return other != null && this.Identifier == other.Identifier;
		}

		public override bool Equals(object obj)
		{
			return obj is CecilType other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.reference.GetHashCode();
		}

		public bool IsUsing(IMethod method)
		{
			return this.Methods.Any(candidate => candidate.IsUsing(method));
		}

		public bool IsUsing(IType type)
		{
			var usedInAttributes = this.Attributes.Any(attribute => type.Equals(attribute.Type));
			var usedInBase = this.BaseOrNull != null && type.Equals(this.BaseOrNull);
			var usedInFields = this.Fields.Any(field => type.Equals(field.Type));
			var usedInInterfaces = this.Interfaces.Any(type.Equals);
			var usedInMethods = this.Methods.Any(method => method.IsUsing(type));
			var usedInNestedTypes = this.NestedTypes.Any(type.Equals);
			var usedInParameters = this.Parameters.Any(parameter => parameter.Constraints.Any(type.Equals));

			return usedInAttributes || usedInBase || usedInFields || usedInInterfaces || usedInMethods ||
			       usedInNestedTypes || usedInParameters;
		}

		public override string ToString()
		{
			return $"{{Type={this.Identifier}}}";
		}
	}
}
