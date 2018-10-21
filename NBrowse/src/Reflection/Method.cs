using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection
{
	public struct Method
	{
		public IEnumerable<Argument> Arguments => _method.Parameters.Select(argument => new Argument(argument));
		public IEnumerable<Attribute> Attributes => _method.CustomAttributes.Select(attribute => new Attribute(attribute));
		public Binding Binding => _method.IsConstructor ? Binding.Constructor : (_method.IsStatic ? Binding.Static : Binding.Dynamic);
		public string Identifier => $"{Parent.Identifier}.{Name}({string.Join(", ", Arguments.Select(argument => argument.Identifier))})";
		public Inheritance Inheritance => _method.IsAbstract ? Inheritance.Abstract : (_method.IsFinal ? Inheritance.Final : (_method.IsVirtual ? Inheritance.Virtual : Inheritance.Actual));
		public string Name => _method.Name;
		public IEnumerable<Parameter> Parameters => _method.GenericParameters.Select(parameter => new Parameter(parameter));
		public Type Parent => new Type(_method.DeclaringType);
		public Type ReturnType => new Type(_method.ReturnType);
		public Visibility Visibility => _method.IsPublic ? Visibility.Public : (_method.IsPrivate ? Visibility.Private : (_method.IsFamily ? Visibility.Protected : Visibility.Internal));

		private readonly MethodDefinition _method;

		public Method(MethodDefinition method)
		{
			_method = method;
		}

		public bool IsUsing(Type type)
		{
			var usedInArguments = Arguments.Any(argument => type.Equals(argument.Type));
			var usedInAttributes = Attributes.Any(attribute => type.Equals(attribute.Type));
			var usedInBody = MatchInstruction(instruction => instruction.Operand is TypeReference operand && type.Equals(new Type(operand)));
			var usedInReturn = type.Equals(ReturnType);

			// FIXME: should also detect for generic parameter guards
			return usedInArguments || usedInAttributes || usedInBody || usedInReturn;
		}

		public override string ToString()
		{
			return $"{{Method={Identifier}}}";
		}

		private bool MatchInstruction(Func<Instruction, bool> predicate)
		{
			return _method.Body != null && _method.Body.Instructions.Any(predicate);
		}
	}
}