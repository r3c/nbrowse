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
		public IEnumerable<Argument> Arguments => _method.Parameters.Select(argument => new Argument(argument));

		[JsonIgnore]
		public IEnumerable<Attribute> Attributes => _method.CustomAttributes.Select(attribute => new Attribute(attribute));

		[JsonConverter(typeof(StringEnumConverter))]
		public Binding Binding => _method.IsConstructor ? Binding.Constructor : (_method.IsStatic ? Binding.Static : Binding.Instance);

		public string Identifier => $"{Parent.Identifier}.{Name}({string.Join(", ", Arguments.Select(argument => argument.Identifier))})";

		[JsonConverter(typeof(StringEnumConverter))]
		public Implementation Implementation => _method.IsAbstract ? Implementation.Abstract : (_method.IsFinal ? Implementation.Final : (_method.IsVirtual ? Implementation.Virtual : Implementation.Concrete));

		public string Name => _method.Name;

		[JsonIgnore]
		public IEnumerable<Parameter> Parameters => _method.GenericParameters.Select(parameter => new Parameter(parameter));

		[JsonIgnore]
		public Type Parent => new Type(_method.DeclaringType);

		public Type ReturnType => new Type(_method.ReturnType);

		[JsonConverter(typeof(StringEnumConverter))]
		public Visibility Visibility => _method.IsPublic ? Visibility.Public : (_method.IsPrivate ? Visibility.Private : (_method.IsFamily ? Visibility.Protected : Visibility.Internal));

		private readonly MethodDefinition _method;

		public Method(MethodDefinition method)
		{
			_method = method;
		}

		public bool Equals(Method other)
		{
			// FIXME: https://cdn-images-1.medium.com/max/1200/1*snTXFElFuQLSFDnvZKJ6IA.png
			return _method.MetadataToken.RID == other._method.MetadataToken.RID;
		}

		public bool IsCalling(Method method)
		{
			Func<OpCode, bool> isMethodCall = opCode => opCode == OpCodes.Call || opCode == OpCodes.Callvirt;
			Func<object, bool> isSameReference = operand => operand is MethodReference reference && method._method.MetadataToken.RID == reference.MetadataToken.RID;

			return MatchInstruction(instruction => isMethodCall(instruction.OpCode) && isSameReference(instruction.Operand));
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
			return _method.Body != null && _method.Body.Instructions.Any(predicate);
		}
	}
}