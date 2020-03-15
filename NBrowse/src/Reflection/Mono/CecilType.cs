using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilType : Type
	{
		public override IEnumerable<Attribute> Attributes =>
			this.definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute, this.project)) ??
			Array.Empty<CecilAttribute>();

		public override Type BaseOrNull => this.definition?.BaseType != null
			? new CecilType(this.definition.BaseType, this.project)
			: default(Type);

		public override Type ElementOrNull => this.reference is ArrayType arrayType
			? (arrayType.ElementType != null ? new CecilType(arrayType.ElementType, this.project) : default(Type))
			: (this.reference is PointerType pointerType
				? (pointerType.ElementType != null
					? new CecilType(pointerType.ElementType, this.project)
					: default(Type))
				: (this.reference is ByReferenceType byReferenceType
					? (byReferenceType.ElementType != null
						? new CecilType(byReferenceType.ElementType, this.project)
						: default(Type))
					: default));

		public override IEnumerable<Field> Fields =>
			this.definition?.Fields.Select(field => new CecilField(field, this.project)) ?? Array.Empty<CecilField>();

		public override string Identifier =>
			$"{this.Namespace}{(string.IsNullOrEmpty(this.Namespace) ? "" : ".")}{this.Name}";

		public override Definition Definition => this.definition == null
			? Definition.Unknown
			: (this.definition.IsAbstract
				? Definition.Abstract
				: (this.definition.IsSealed
					? Definition.Final
					: Definition.Virtual));

		public override IEnumerable<Type> Interfaces =>
			this.definition?.Interfaces.Select(i => new CecilType(i.InterfaceType, this.project)) ??
			Array.Empty<CecilType>();

		public override IEnumerable<Method> Methods =>
			this.definition?.Methods.Select(method => new CecilMethod(method, this.project)) ??
			Array.Empty<CecilMethod>();

		public override Model Model => this.reference.IsValueType
			? (this.definition?.IsEnum ?? false ? Model.Enumeration : Model.Structure)
			: (this.definition?.IsInterface ?? false
				? Model.Interface
				: (this.reference.IsArray
					? Model.Array
					: (this.reference.IsPointer
						? Model.Pointer
						: (this.reference.IsByReference ? Model.Reference : Model.Class))));

		public override string Name => this.reference.IsNested
			? $"{new CecilType(this.reference.DeclaringType, this.project).Name}+{this.reference.Name}"
			: this.reference.Name;

		public override string Namespace => this.reference.IsNested
			? new CecilType(this.reference.DeclaringType, this.project).Namespace
			: this.reference.Namespace;

		public override IEnumerable<Type> NestedTypes =>
			this.definition?.NestedTypes.Select(type => new CecilType(type, this.project)) ?? Array.Empty<CecilType>();

		public override IEnumerable<Parameter> Parameters =>
			this.reference.GenericParameters.Select(parameter => new CecilParameter(parameter, this.project));

		public override Assembly Parent => this.definition != null
			? new CecilAssembly(this.definition.Module.Assembly, this.project)
			: CecilAssembly.Empty;

		public override Visibility Visibility => this.definition == null
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
		private readonly Project project;
		private readonly TypeReference reference;

		public CecilType(TypeReference reference, Project project)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));

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
			this.project = project;
			this.reference = reference;
		}

		public override bool Equals(Type other)
		{
			return !object.ReferenceEquals(other, null) && this.Namespace == other.Namespace &&
			       this.Name == other.Name && this.Parameters.SequenceEqual(other.Parameters);
		}
	}
}
