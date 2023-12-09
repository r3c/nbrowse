using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilParameter : Parameter
{
    public override IEnumerable<Type> Constraints =>
        _parameter.Constraints.Select(constraint => new CecilType(constraint.ConstraintType, _project));

    public override bool HasDefaultConstructorConstraint => _parameter.HasDefaultConstructorConstraint;

    public override bool HasReferenceTypeConstraint => _parameter.HasReferenceTypeConstraint;

    public override bool HasValueTypeConstraint => _parameter.HasNotNullableValueTypeConstraint;

    public override string Identifier => _parameter.FullName + (_parameter.Constraints.Count > 0
        ? " : " + string.Join(", ", Constraints)
        : string.Empty);

    public override string Name => _parameter.Name;

    public override Variance Variance => _parameter.IsContravariant
        ? Variance.Contravariant
        : _parameter.IsCovariant
            ? Variance.Covariant
            : Variance.Invariant;

    private readonly GenericParameter _parameter;
    private readonly Project _project;

    public CecilParameter(GenericParameter parameter, Project project)
    {
        _parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        _project = project;
    }

    public override bool Equals(Parameter other)
    {
        return !ReferenceEquals(other, null) && HasDefaultConstructorConstraint == other.HasDefaultConstructorConstraint &&
               Name == other.Name && Variance == other.Variance &&
               Constraints.SequenceEqual(other.Constraints);
    }
}