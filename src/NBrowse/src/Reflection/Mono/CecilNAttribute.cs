using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilNAttribute : NAttribute
{
    public override IEnumerable<object> Arguments =>
        _attribute.ConstructorArguments.Select(argument => argument.Value);

    public override NMethod Constructor => new CecilNMethod(_attribute.Constructor, _nProject);

    public override string Identifier => $"{NType.Identifier}({string.Join(", ", Arguments)})";

    public override NType NType => new CecilNType(_attribute.AttributeType, _nProject);

    private readonly CustomAttribute _attribute;
    private readonly NProject _nProject;

    public CecilNAttribute(CustomAttribute attribute, NProject nProject)
    {
        _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        _nProject = nProject;
    }

    public override bool Equals(NAttribute other)
    {
        return !ReferenceEquals(other, null) && Constructor == other.Constructor &&
               NType == other.NType && Arguments.SequenceEqual(other.Arguments);
    }
}