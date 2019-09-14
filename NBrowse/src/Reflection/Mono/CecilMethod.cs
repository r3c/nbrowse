using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilMethod : IMethod
	{
		public IEnumerable<IArgument> Arguments =>
			this.reference.Parameters.Select(argument => new CecilArgument(argument));

		public IEnumerable<IAttribute> Attributes =>
			this.definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute)) ??
			Array.Empty<CecilAttribute>();

		public Binding Binding => this.definition == null
			? Binding.Unknown
			: (this.definition.IsConstructor
				? Binding.Constructor
				: (this.definition.IsStatic
					? Binding.Static
					: Binding.Instance));

		public string Identifier => $"{this.Parent.Identifier}.{this.Name}({string.Join(", ", this.Arguments.Select(argument => argument.Identifier))})";

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

		public IEnumerable<IParameter> Parameters =>
			this.reference.GenericParameters.Select(parameter => new CecilParameter(parameter));

		public IType Parent => new CecilType(this.reference.DeclaringType);

		public IType ReturnType => new CecilType(this.reference.ReturnType);

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

		public CecilMethod(MethodDefinition definition)
		{
			this.definition = definition;
			this.reference = definition;
		}

		public CecilMethod(MethodReference reference)
		{
			this.definition = reference.IsDefinition || reference.Module.AssemblyResolver != null ? reference.Resolve() : null;
			this.reference = reference;
		}

		public bool Equals(IMethod other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return other != null && this.Identifier == other.Identifier;
		}

		public override bool Equals(object obj)
		{
			return obj is CecilMethod other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.reference.GetHashCode();
		}

		public bool IsUsing(IMethod method)
		{
			return this.MatchInstruction(instruction =>
				instruction.Operand is MethodReference reference && method.Equals(new CecilMethod(reference)));
		}

		public bool IsUsing(IType type)
		{
			var usedInArguments = this.Arguments.Any(argument => type.Equals(argument.Type));
			var usedInAttributes = this.Attributes.Any(attribute => type.Equals(attribute.Type));
			var usedInBody = this.MatchInstruction(instruction => instruction.Operand is TypeReference operand && type.Equals(new CecilType(operand)));
			var usedInParameters = this.Parameters.Any(parameter => parameter.Constraints.Any(type.Equals));
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