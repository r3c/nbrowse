using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilAttribute : Attribute
{
    public override IEnumerable<object> Arguments =>
        _attribute.ConstructorArguments.Select(argument => argument.Value);

    public override Method Constructor => new CecilMethod(_attribute.Constructor, _project);

    public override string Identifier => $"{Type.Identifier}({string.Join(", ", Arguments)})";

    public override Type Type => new CecilType(_attribute.AttributeType, _project);

    private readonly CustomAttribute _attribute;
    private readonly Project _project;

    public CecilAttribute(CustomAttribute attribute, Project project)
    {
        _attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        _project = project;
    }

    public override bool Equals(Attribute other)
    {
        return !ReferenceEquals(other, null) && Constructor == other.Constructor &&
               Type == other.Type && Arguments.SequenceEqual(other.Arguments);
    }
}