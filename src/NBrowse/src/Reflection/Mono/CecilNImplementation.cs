using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace NBrowse.Reflection.Mono;

internal class CecilNImplementation : NImplementation
{
    public override NMethod Parent { get; }

    public override IEnumerable<NMethod> ReferencedMethods => GetReferencedMethods(_body.Instructions);

    public override IEnumerable<NType> ReferencedTypes => GetReferencedTypes(_body.Instructions);

    private readonly MethodBody _body;
    private readonly NProject _nProject;

    public CecilNImplementation(NMethod parent, MethodBody body, NProject nProject)
    {
        _body = body;
        _nProject = nProject;

        Parent = parent;
    }

    private IEnumerable<NMethod> GetReferencedMethods(IEnumerable<Instruction> instructions)
    {
        foreach (var instruction in instructions)
            switch (instruction.Operand)
            {
                case MethodReference method:
                    yield return new CecilNMethod(method, _nProject);
                    break;
            }
    }

    private IEnumerable<NType> GetReferencedTypes(IEnumerable<Instruction> instructions)
    {
        foreach (var instruction in instructions)
            switch (instruction.Operand)
            {
                case FieldReference field:
                    yield return new CecilNType(field.DeclaringType, _nProject);
                    break;

                case MethodReference method:
                    yield return new CecilNType(method.DeclaringType, _nProject);
                    break;

                case PropertyReference property:
                    yield return new CecilNType(property.DeclaringType, _nProject);
                    break;

                case TypeReference type:
                    yield return new CecilNType(type, _nProject);
                    break;
            }
    }
}