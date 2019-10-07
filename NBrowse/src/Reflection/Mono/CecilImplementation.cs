using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilImplementation : IImplementation
	{
		public IEnumerable<IMethod> ReferencedMethods => this.GetReferencedMethods(this.body.Instructions);

		public IEnumerable<IType> ReferencedTypes => this.GetReferencedTypes(this.body.Instructions);

		private readonly MethodBody body;
		private readonly IAssembly parent;

		public CecilImplementation(MethodBody body, IAssembly parent)
		{
			this.body = body;
			this.parent = parent;
		}

		private IEnumerable<IMethod> GetReferencedMethods(IEnumerable<Instruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				switch (instruction.Operand)
				{
					case MethodReference method:
						yield return new CecilMethod(method, this.parent);
						break;
				}
			}
		}

		private IEnumerable<IType> GetReferencedTypes(IEnumerable<Instruction> instructions)
		{
			foreach (var instruction in instructions)
			{
				switch (instruction.Operand)
				{
					case FieldReference field:
						yield return new CecilType(field.DeclaringType, this.parent);
						break;

					case MethodReference method:
						yield return new CecilType(method.DeclaringType, this.parent);
						break;

					case PropertyReference property:
						yield return new CecilType(property.DeclaringType, this.parent);
						break;

					case TypeReference type:
						yield return new CecilType(type, this.parent);
						break;
				}
			}
		}
	}
}