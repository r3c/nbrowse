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
		public string Identifier => $"{Parent.Identifier}.{Name}({string.Join(", ", Arguments.Select(argument => argument.Identifier))})";
		public string Name => _method.Name;
		public Type Parent => new Type(_method.DeclaringType);

		private readonly MethodDefinition _method;

		public Method(MethodDefinition method)
		{
			_method = method;
		}

		public bool IsUsing(Type type)
		{
			var usedInArguments = Arguments.Any(argument => type.Equals(argument.Type));
			var usedInBody = MatchInstruction(instruction => instruction.Operand is TypeReference operand && type.Equals(new Type(operand)));

			// FIXME: should also detect for custom attributes & generic parameter guards
			return usedInArguments || usedInBody;
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