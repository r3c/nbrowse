using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
	public struct MethodModel
	{
		public IEnumerable<ArgumentModel> Arguments => _method.Parameters.Select(argument => new ArgumentModel(argument));
		public string FullName => $"{Parent.FullName}.{Name}({string.Join(", ", Arguments.Select(argument => argument.FullName))})";
		public string Name => _method.Name;
		public TypeModel Parent => new TypeModel(_method.DeclaringType);

		private readonly MethodDefinition _method;

		public MethodModel(MethodDefinition method)
		{
			_method = method;
		}

		public bool IsUsing(TypeModel type)
		{
			var usedInArguments = Arguments.Any(argument => type.Equals(argument.Type));
			var usedInBody = MatchInstruction(instruction => instruction.Operand is TypeReference operand && type.Equals(new TypeModel(operand)));

			// FIXME: should also detect for custom attributes & generic parameter guards
			return usedInArguments || usedInBody;
		}

		public override string ToString()
		{
			return $"{{Method={FullName}}}";
		}

		private bool MatchInstruction(Func<Instruction, bool> predicate)
		{
			return _method.Body != null && _method.Body.Instructions.Any(predicate);
		}
	}
}