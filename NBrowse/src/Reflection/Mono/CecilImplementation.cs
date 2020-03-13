using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilImplementation : Implementation
	{
		public override Method Parent => new CecilMethod(this.body.Method, this.parent);

		public override IEnumerable<Method> ReferencedMethods => this.GetReferencedMethods(this.body.Instructions);

		public override IEnumerable<Type> ReferencedTypes => this.GetReferencedTypes(this.body.Instructions);

		private readonly MethodBody body;
		private readonly Assembly parent;

		public CecilImplementation(MethodBody body, Assembly parent)
		{
			this.body = body;
			this.parent = parent;
		}

		private IEnumerable<Method> GetReferencedMethods(IEnumerable<Instruction> instructions)
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

		private IEnumerable<Type> GetReferencedTypes(IEnumerable<Instruction> instructions)
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