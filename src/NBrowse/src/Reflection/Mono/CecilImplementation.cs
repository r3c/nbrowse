using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection.Mono;

internal class CecilImplementation : Implementation
{
    public override Method Parent { get; }

    public override IEnumerable<Method> ReferencedMethods => GetReferencedMethods(_body.Instructions);

    public override IEnumerable<Type> ReferencedTypes => GetReferencedTypes(_body.Instructions);

    private readonly MethodBody _body;
    private readonly Project _project;

    public CecilImplementation(Method parent, MethodBody body, Project project)
    {
        _body = body;
        _project = project;

        Parent = parent;
    }

    private IEnumerable<Method> GetReferencedMethods(IEnumerable<Instruction> instructions)
    {
        foreach (var instruction in instructions)
            switch (instruction.Operand)
            {
                case MethodReference method:
                    yield return new CecilMethod(method, _project);
                    break;
            }
    }

    private IEnumerable<Type> GetReferencedTypes(IEnumerable<Instruction> instructions)
    {
        foreach (var instruction in instructions)
            switch (instruction.Operand)
            {
                case FieldReference field:
                    yield return new CecilType(field.DeclaringType, _project);
                    break;

                case MethodReference method:
                    yield return new CecilType(method.DeclaringType, _project);
                    break;

                case PropertyReference property:
                    yield return new CecilType(property.DeclaringType, _project);
                    break;

                case TypeReference type:
                    yield return new CecilType(type, _project);
                    break;
            }
    }
}