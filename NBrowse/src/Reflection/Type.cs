using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NBrowse.Selection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public struct Type : IEquatable<Type>
	{
		[JsonIgnore]
		public IEnumerable<Attribute> Attributes => _definition?.CustomAttributes.Select(attribute => new Attribute(attribute)) ?? Array.Empty<Attribute>();

		[JsonIgnore]
		public Type? Base => _definition != null && _definition.BaseType != null ? new Type(_definition.BaseType) as Type? : null;

		[JsonIgnore]
		public IEnumerable<Field> Fields => _definition?.Fields.Select(field => new Field(field)) ?? Array.Empty<Field>();

		public string Identifier => $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";

		[JsonConverter(typeof(StringEnumConverter))]
		public Implementation Implementation => _definition == null
			? Implementation.Unknown
			: (_definition.IsAbstract
				? Implementation.Abstract
				: (_definition.IsSealed
					? Implementation.Final
					: Implementation.Virtual));

		[JsonIgnore]
		public IEnumerable<Type> Interfaces => _definition?.Interfaces.Select(i => new Type(i.InterfaceType)) ?? Array.Empty<Type>();

		[JsonIgnore]
		public IEnumerable<Method> Methods => _definition?.Methods.Select(method => new Method(method)) ?? Array.Empty<Method>();

		[JsonConverter(typeof(StringEnumConverter))]
		public Model Model => _definition == null
			? Model.Unknown
			: (_definition.IsEnum
				? Model.Enumeration
				: (_definition.IsInterface
					? Model.Interface
					: (_definition.IsValueType
						? Model.Structure
						: Model.Class)));

		public string Name => _reference.IsNested ? $"{new Type(_reference.DeclaringType).Name}+{_reference.Name}" : _reference.Name;

		public string Namespace => _reference.IsNested ? new Type(_reference.DeclaringType).Namespace : _reference.Namespace;

		[JsonIgnore]
		public IEnumerable<Type> NestedTypes => _definition?.NestedTypes.Select(type => new Type(type)) ?? Array.Empty<Type>();

		[JsonIgnore]
		public IEnumerable<Parameter> Parameters => _definition?.GenericParameters.Select(parameter => new Parameter(parameter)) ?? Array.Empty<Parameter>();

		[JsonIgnore]
		public Assembly Parent => new Assembly(_reference.Module.Assembly);

		[JsonConverter(typeof(StringEnumConverter))]
		public Visibility Visibility => _definition == null
			? Visibility.Unknown
			: (_definition.IsNested
				? (_definition.IsNestedPublic
					? Visibility.Public
					: (_definition.IsNestedFamily
						? Visibility.Protected
						: (_definition.IsNestedPrivate
							? Visibility.Private
							: Visibility.Internal)))
				: (_definition.IsPublic
					? Visibility.Public
					: (_definition.IsNotPublic
						? Visibility.Private
						: Visibility.Internal)));

		private readonly TypeDefinition _definition;
		private readonly TypeReference _reference;

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
			_definition = definition;
			_reference = definition;
		}

		public Type(TypeReference reference)
		{
			_definition = reference.IsDefinition || reference.Module.AssemblyResolver != null ? reference.Resolve() : null;
			_reference = reference;
		}

		public bool Equals(Type other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return Identifier == other.Identifier;
		}

		public override bool Equals(object o)
		{
			return o is Type other && Equals(other);
		}

		public override int GetHashCode()
		{
			return _reference.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Type={Identifier}}}";
		}
	}
}