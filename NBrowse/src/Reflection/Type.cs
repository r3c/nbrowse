using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public struct Type : IEquatable<Type>
	{
		[JsonIgnore]
		public IEnumerable<Attribute> Attributes => _definition != null ? _definition.CustomAttributes.Select(attribute => new Attribute(attribute)) : Array.Empty<Attribute>();
		[JsonIgnore]
		public Type? Base => _definition != null && _definition.BaseType != null ? new Type(_definition.BaseType) as Type? : null;
		[JsonIgnore]
		public IEnumerable<Field> Fields => _definition != null ? _definition.Fields.Select(field => new Field(field)) : Array.Empty<Field>();
		public string Identifier => $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";
		[JsonConverter(typeof(StringEnumConverter))]
		public Implementation Implementation => _definition != null && _definition.IsAbstract ? Implementation.Abstract : (_definition != null && _definition.IsSealed ? Implementation.Final : Implementation.Virtual);
		[JsonIgnore]
		public IEnumerable<Method> Methods => _definition != null ? _definition.Methods.Select(method => new Method(method)) : Array.Empty<Method>();
		[JsonConverter(typeof(StringEnumConverter))]
		public Model Model => _definition.IsEnum ? Model.Enumeration : (_definition.IsInterface ? Model.Interface : (_definition.IsValueType ? Model.Structure : Model.Class));
		public string Name => _definition != null && _definition.IsNested ? $"{new Type(_definition.DeclaringType).Name}+{_reference.Name}" : _reference.Name;
		public string Namespace => _definition == null ? string.Empty : (_definition.IsNested ? new Type(_definition.DeclaringType).Namespace : _definition.Namespace);
		[JsonIgnore]
		public IEnumerable<Parameter> Parameters => _definition != null ? _definition.GenericParameters.Select(parameter => new Parameter(parameter)) : Array.Empty<Parameter>();
		[JsonIgnore]
		public Assembly Parent => new Assembly(_reference.Module.Assembly);

		[JsonConverter(typeof(StringEnumConverter))]
		public Visibility Visibility
		{
			get
			{
				if (_definition == null)
					return Visibility.Public;

				if (_definition.IsNested)
					return _definition.IsNestedPublic ? Visibility.Public : (_definition.IsNestedFamily ? Visibility.Protected : (_definition.IsNestedPrivate ? Visibility.Private : Visibility.Internal));

				return _definition.IsPublic ? Visibility.Public : (_definition.IsNotPublic ? Visibility.Private : Visibility.Internal);
			}
		}

		private readonly TypeDefinition _definition;
		private readonly TypeReference _reference;

		public Type(TypeDefinition definition)
		{
			_definition = definition;
			_reference = definition;
		}

		public Type(TypeReference reference)
		{
			_definition = reference.IsDefinition ? reference.Resolve() : null;
			_reference = reference;
		}

		public bool Equals(Type other)
		{
			// FIXME: most probably inaccurate, waiting for https://github.com/jbevain/cecil/pull/394
			return _reference.FullName == other._reference.FullName;
		}

		public override string ToString()
		{
			return $"{{Type={Identifier}}}";
		}
	}
}