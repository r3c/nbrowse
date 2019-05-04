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
		public IEnumerable<Argument> Arguments => this.reference.Parameters.Select(argument => new Argument(argument));

		[JsonIgnore]
		public IEnumerable<Attribute> Attributes => this.definition?.CustomAttributes.Select(attribute => new Attribute(attribute)) ?? Array.Empty<Attribute>();

		[JsonConverter(typeof(StringEnumConverter))]
		public Binding Binding => this.definition == null
			? Binding.Unknown
			: (this.definition.IsConstructor
				? Binding.Constructor
				: (this.definition.IsStatic
					? Binding.Static
					: Binding.Instance));

		public string Identifier => $"{this.Parent.Identifier}.{this.Name}({string.Join(", ", this.Arguments.Select(argument => argument.Identifier))})";

		[JsonConverter(typeof(StringEnumConverter))]
		public Implementation Implementation => this.definition == null
			? Implementation.Unknown
			: (this.definition.IsAbstract
				? Implementation.Abstract
				: (this.definition.IsFinal
					? Implementation.Final
					: (this.definition.IsVirtual
						? Implementation.Virtual
						: Implementation.Concrete)));

		public string Name => this.reference.Name;

		[JsonIgnore]
		public IEnumerable<Parameter> Parameters => this.reference.GenericParameters.Select(parameter => new Parameter(parameter));

		[JsonIgnore]
		public Type Parent => new Type(this.reference.DeclaringType);

		public Type ReturnType => new Type(this.reference.ReturnType);

		[JsonConverter(typeof(StringEnumConverter))]
		public Visibility Visibility => this.definition == null
			? Visibility.Unknown
			: (this.definition.IsPublic
				? Visibility.Public
				: (this.definition.IsPrivate
					? Visibility.Private
					: (this.definition.IsFamily
						? Visibility.Protected
						: Visibility.Internal)));

		private readonly MethodDefinition definition;
		private readonly MethodReference reference;

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
			this.definition = definition;
			this.reference = definition;
		}

		public Method(MethodReference reference)
		{
			this.definition = reference.IsDefinition || reference.Module.AssemblyResolver != null ? reference.Resolve() : null;
			this.reference = reference;
		}

		public bool Equals(Method other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return this.Identifier == other.Identifier;
		}

		public override bool Equals(object o)
		{
			return o is Method other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.reference.GetHashCode();
		}

		public bool IsUsing(Method method)
		{
			return this.MatchInstruction(instruction => instruction.Operand is MethodReference reference && method == new Method(reference));
		}

		public bool IsUsing(Type type)
		{
			var usedInArguments = this.Arguments.Any(argument => type.Equals(argument.Type));
			var usedInAttributes = this.Attributes.Any(attribute => type.Equals(attribute.Type));
			var usedInBody = this.MatchInstruction(instruction => instruction.Operand is TypeReference operand && type.Equals(new Type(operand)));
			var usedInParameters = this.Parameters.Any(parameter => parameter.Constraints.Any(constraint => type.Equals(constraint)));
			var usedInReturn = type.Equals(this.ReturnType);

			return usedInArguments || usedInAttributes || usedInBody || usedInParameters || usedInReturn;
		}

		public override string ToString()
		{
			return $"{{Method={this.Identifier}}}";
		}

		private bool MatchInstruction(Func<Instruction, bool> predicate)
		{
			return this.definition != null && this.definition.Body != null && this.definition.Body.Instructions.Any(predicate);
		}
	}
}