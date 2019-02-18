using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NBrowse.Reflection
{
	public struct Method : IEquatable<Method>
	{
		[JsonIgnore]
		public IEnumerable<Argument> Arguments => _reference.Parameters.Select(argument => new Argument(argument));

		[JsonIgnore]
		public IEnumerable<Attribute> Attributes => _definition?.CustomAttributes.Select(attribute => new Attribute(attribute)) ?? Array.Empty<Attribute>();

		[JsonConverter(typeof(StringEnumConverter))]
		public Binding Binding => _definition == null
			? Binding.Unknown
			: (_definition.IsConstructor
				? Binding.Constructor
				: (_definition.IsStatic
					? Binding.Static
					: Binding.Instance));

		public string Identifier => $"{Parent.Identifier}.{Name}({string.Join(", ", Arguments.Select(argument => argument.Identifier))})";

		[JsonConverter(typeof(StringEnumConverter))]
		public Implementation Implementation => _definition == null
			? Implementation.Unknown
			: (_definition.IsAbstract
				? Implementation.Abstract
				: (_definition.IsFinal
					? Implementation.Final
					: (_definition.IsVirtual
						? Implementation.Virtual
						: Implementation.Concrete)));

		public string Name => _reference.Name;

		[JsonIgnore]
		public IEnumerable<Parameter> Parameters => _reference.GenericParameters.Select(parameter => new Parameter(parameter));

		[JsonIgnore]
		public Type Parent => new Type(_reference.DeclaringType);

		public Type ReturnType => new Type(_reference.ReturnType);

		[JsonConverter(typeof(StringEnumConverter))]
		public Visibility Visibility => _definition == null
			? Visibility.Unknown
			: (_definition.IsPublic
				? Visibility.Public
				: (_definition.IsPrivate
					? Visibility.Private
					: (_definition.IsFamily
						? Visibility.Protected
						: Visibility.Internal)));

		private readonly MethodDefinition _definition;
		private readonly MethodReference _reference;

		public static bool operator ==(Method lhs, Method rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Method lhs, Method rhs)
		{
			return !lhs.Equals(rhs);
		}

		public Method(MethodDefinition definition)
		{
			_definition = definition;
			_reference = definition;
		}

		public Method(MethodReference reference)
		{
			_definition = reference.IsDefinition || reference.Module.AssemblyResolver != null ? reference.Resolve() : null;
			_reference = reference;
		}

		public bool Equals(Method other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return Identifier == other.Identifier;
		}

		public override bool Equals(object o)
		{
			return o is Method other && Equals(other);
		}

		public override int GetHashCode()
		{
			return _reference.GetHashCode();
		}

		public bool IsUsing(Method method)
		{
			return MatchInstruction(instruction => instruction.Operand is MethodReference reference && method == new Method(reference));
		}

		public bool IsUsing(Type type)
		{
			var usedInArguments = Arguments.Any(argument => type.Equals(argument.Type));
			var usedInAttributes = Attributes.Any(attribute => type.Equals(attribute.Type));
			var usedInBody = MatchInstruction(instruction => instruction.Operand is TypeReference operand && type.Equals(new Type(operand)));
			var usedInParameters = Parameters.Any(parameter => parameter.Constraints.Any(constraint => type.Equals(constraint)));
			var usedInReturn = type.Equals(ReturnType);

			return usedInArguments || usedInAttributes || usedInBody || usedInParameters || usedInReturn;
		}

		public override string ToString()
		{
			return $"{{Method={Identifier}}}";
		}

		private bool MatchInstruction(Func<Instruction, bool> predicate)
		{
			return _definition != null && _definition.Body != null && _definition.Body.Instructions.Any(predicate);
		}
	}
}