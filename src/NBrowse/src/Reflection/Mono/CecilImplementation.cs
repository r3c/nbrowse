using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection.Mono
{
    internal class CecilImplementation : Implementation
    {
        public override Method Parent { get; }

        public override IEnumerable<Method> ReferencedMethods => this.GetReferencedMethods(this.body.Instructions);

        public override IEnumerable<Type> ReferencedTypes => this.GetReferencedTypes(this.body.Instructions);

        private readonly MethodBody body;
        private readonly Project project;

        public CecilImplementation(Method parent, MethodBody body, Project project)
        {
            this.body = body;
            this.project = project;

            this.Parent = parent;
        }

        private IEnumerable<Method> GetReferencedMethods(IEnumerable<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                switch (instruction.Operand)
                {
                    case MethodReference method:
                        yield return new CecilMethod(method, this.project);
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
                        yield return new CecilType(field.DeclaringType, this.project);
                        break;

                    case MethodReference method:
                        yield return new CecilType(method.DeclaringType, this.project);
                        break;

                    case PropertyReference property:
                        yield return new CecilType(property.DeclaringType, this.project);
                        break;

                    case TypeReference type:
                        yield return new CecilType(type, this.project);
                        break;
                }
            }
        }
    }
}