using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public struct Type : IEquatable<Type>
	{
		[JsonIgnore]
		public IEnumerable<Attribute> Attributes => this.definition?.CustomAttributes.Select(attribute => new Attribute(attribute)) ?? Array.Empty<Attribute>();

		[JsonIgnore]
		public Type? Base => this.definition != null && this.definition.BaseType != null ? new Type(this.definition.BaseType) as Type? : null;

		[JsonIgnore]
		public IEnumerable<Field> Fields => this.definition?.Fields.Select(field => new Field(field)) ?? Array.Empty<Field>();

		public string Identifier => $"{this.Namespace}{(string.IsNullOrEmpty(this.Namespace) ? "" : ".")}{this.Name}";

		[JsonConverter(typeof(StringEnumConverter))]
		public Implementation Implementation => this.definition == null
			? Implementation.Unknown
			: (this.definition.IsAbstract
				? Implementation.Abstract
				: (this.definition.IsSealed
					? Implementation.Final
					: Implementation.Virtual));

		[JsonIgnore]
		public IEnumerable<Type> Interfaces => this.definition?.Interfaces.Select(i => new Type(i.InterfaceType)) ?? Array.Empty<Type>();

		[JsonIgnore]
		public IEnumerable<Method> Methods => this.definition?.Methods.Select(method => new Method(method)) ?? Array.Empty<Method>();

		[JsonConverter(typeof(StringEnumConverter))]
		public Model Model => this.definition == null
			? Model.Unknown
			: (this.definition.IsEnum
				? Model.Enumeration
				: (this.definition.IsInterface
					? Model.Interface
					: (this.definition.IsValueType
						? Model.Structure
						: Model.Class)));

		public string Name => this.reference.IsNested ? $"{new Type(this.reference.DeclaringType).Name}+{this.reference.Name}" : this.reference.Name;

		public string Namespace => this.reference.IsNested ? new Type(this.reference.DeclaringType).Namespace : this.reference.Namespace;

		[JsonIgnore]
		public IEnumerable<Type> NestedTypes => this.definition?.NestedTypes.Select(type => new Type(type)) ?? Array.Empty<Type>();

		[JsonIgnore]
		public IEnumerable<Parameter> Parameters => this.definition?.GenericParameters.Select(parameter => new Parameter(parameter)) ?? Array.Empty<Parameter>();

		[JsonIgnore]
		public Assembly Parent => new Assembly(this.reference.Module.Assembly);

		[JsonConverter(typeof(StringEnumConverter))]
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

		public static bool operator ==(Type lhs, Type rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Type lhs, Type rhs)
		{
			return !lhs.Equals(rhs);
		}

		public Type(TypeDefinition definition)
		{
			this.definition = definition;
			this.reference = definition;
		}

		public Type(TypeReference reference)
		{
			TypeDefinition definition;

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

			this.definition = definition;
			this.reference = reference;
		}

		public bool Equals(Type other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return this.Identifier == other.Identifier;
		}

		public override bool Equals(object o)
		{
			return o is Type other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.reference.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Type={this.Identifier}}}";
		}
	}
}