using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilNField : NField
{
    public override IEnumerable<NAttribute> Attributes =>
        _field.CustomAttributes.Select(attribute => new CecilNAttribute(attribute, _nProject));

    public override NBinding NBinding => _field.IsStatic ? NBinding.Static : NBinding.Instance;

    public override string Identifier => $"{Parent.Identifier}.{Name}";

    public override string Name => _field.Name;

    public override NType Parent => new CecilNType(_field.DeclaringType, _nProject);

    public override NType NType => new CecilNType(_field.FieldType, _nProject);

    public override NVisibility NVisibility => _field.IsPublic
        ? NVisibility.Public
        : _field.IsPrivate
            ? NVisibility.Private
            : _field.IsFamily
                ? NVisibility.Protected
                : NVisibility.Internal;

    private readonly FieldDefinition _field;
    private readonly NProject _nProject;

    public CecilNField(FieldDefinition field, NProject nProject)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
        _nProject = nProject;
    }

    public override bool Equals(NField other)
    {
        return !ReferenceEquals(other, null) && NBinding == other.NBinding && Name == other.Name &&
               Parent == other.Parent && NType == other.NType;
    }
}