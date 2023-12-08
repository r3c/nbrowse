using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
    internal class CecilParameter : Parameter
    {
        public override IEnumerable<Type> Constraints =>
            this.parameter.Constraints.Select(constraint => new CecilType(constraint.ConstraintType, this.project));

        public override bool HasDefaultConstructorConstraint => this.parameter.HasDefaultConstructorConstraint;

        public override bool HasReferenceTypeConstraint => this.parameter.HasReferenceTypeConstraint;

        public override bool HasValueTypeConstraint => this.parameter.HasNotNullableValueTypeConstraint;

        public override string Identifier => this.parameter.FullName + (this.parameter.Constraints.Count > 0
                                                 ? " : " + string.Join(", ", this.Constraints)
                                                 : string.Empty);

        public override string Name => this.parameter.Name;

        public override Variance Variance => this.parameter.IsContravariant
            ? Variance.Contravariant
            : (this.parameter.IsCovariant ? Variance.Covariant : Variance.Invariant);

        private readonly GenericParameter parameter;
        private readonly Project project;

        public CecilParameter(GenericParameter parameter, Project project)
        {
            this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            this.project = project;
        }

        public override bool Equals(Parameter other)
        {
            return !object.ReferenceEquals(other, null) && this.HasDefaultConstructorConstraint == other.HasDefaultConstructorConstraint &&
                   this.Name == other.Name && this.Variance == other.Variance &&
                   this.Constraints.SequenceEqual(other.Constraints);
        }
    }
}