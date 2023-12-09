using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilNParameter : NParameter
{
    public override IEnumerable<NType> Constraints =>
        _parameter.Constraints.Select(constraint => new CecilNType(constraint.ConstraintType, _nProject));

    public override bool HasDefaultConstructorConstraint => _parameter.HasDefaultConstructorConstraint;

    public override bool HasReferenceTypeConstraint => _parameter.HasReferenceTypeConstraint;

    public override bool HasValueTypeConstraint => _parameter.HasNotNullableValueTypeConstraint;

    public override string Identifier => _parameter.FullName + (_parameter.Constraints.Count > 0
        ? " : " + string.Join(", ", Constraints)
        : string.Empty);

    public override string Name => _parameter.Name;

    public override NVariance NVariance => _parameter.IsContravariant
        ? NVariance.Contravariant
        : _parameter.IsCovariant
            ? NVariance.Covariant
            : NVariance.Invariant;

    private readonly GenericParameter _parameter;
    private readonly NProject _nProject;

    public CecilNParameter(GenericParameter parameter, NProject nProject)
    {
        _parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        _nProject = nProject;
    }

    public override bool Equals(NParameter other)
    {
        return !ReferenceEquals(other, null) &&
               HasDefaultConstructorConstraint == other.HasDefaultConstructorConstraint &&
               Name == other.Name && NVariance == other.NVariance &&
               Constraints.SequenceEqual(other.Constraints);
    }
}