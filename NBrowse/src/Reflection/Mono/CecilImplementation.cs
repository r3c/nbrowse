using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilImplementation : IImplementation
	{
		public IEnumerable<IMethod> ReferencedMethods => CecilImplementation.GetReferencedMethods(this.body.Instructions);

		public IEnumerable<IType> ReferencedTypes => CecilImplementation.GetReferencedTypes(this.body.Instructions);

		private readonly MethodBody body;

		public CecilImplementation(MethodBody body)
		{
			this.body = body;
		}

		private static IEnumerable<IMethod> GetReferencedMethods(IEnumerable<Instruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				switch (instruction.Operand)
				{
					case MethodReference method:
						yield return new CecilMethod(method);
						break;
				}
			}
		}

		private static IEnumerable<IType> GetReferencedTypes(IEnumerable<Instruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				switch (instruction.Operand)
				{
					case FieldReference field:
						yield return new CecilType(field.DeclaringType);
						break;

					case MethodReference method:
						yield return new CecilType(method.DeclaringType);
						break;

					case PropertyReference property:
						yield return new CecilType(property.DeclaringType);
						break;

					case TypeReference type:
						yield return new CecilType(type);
						break;
				}
			}
		}
	}
}