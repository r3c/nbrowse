using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilField : Field
{
    public override IEnumerable<Attribute> Attributes =>
        _field.CustomAttributes.Select(attribute => new CecilAttribute(attribute, _project));

    public override Binding Binding => _field.IsStatic ? Binding.Static : Binding.Instance;

    public override string Identifier => $"{Parent.Identifier}.{Name}";

    public override string Name => _field.Name;

    public override Type Parent => new CecilType(_field.DeclaringType, _project);

    public override Type Type => new CecilType(_field.FieldType, _project);

    public override Visibility Visibility => _field.IsPublic
        ? Visibility.Public
        : _field.IsPrivate
            ? Visibility.Private
            : _field.IsFamily
                ? Visibility.Protected
                : Visibility.Internal;

    private readonly FieldDefinition _field;
    private readonly Project _project;

    public CecilField(FieldDefinition field, Project project)
    {
        _field = field ?? throw new ArgumentNullException(nameof(field));
        _project = project;
    }

    public override bool Equals(Field other)
    {
        return !ReferenceEquals(other, null) && Binding == other.Binding && Name == other.Name &&
               Parent == other.Parent && Type == other.Type;
    }
}