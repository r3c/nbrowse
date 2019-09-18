using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
    internal class CecilParameter : IParameter
    {
        public IEnumerable<IType> Constraints =>
            this.parameter.Constraints.Select(constraint => new CecilType(constraint.ConstraintType));

        public bool HasDefaultConstructor => this.parameter.HasDefaultConstructorConstraint;

        public string Identifier => this.parameter.FullName;

        public string Name => this.parameter.Name;

        public Variance Variance => this.parameter.IsContravariant
            ? Variance.Contravariant
            : (this.parameter.IsCovariant ? Variance.Covariant : Variance.Invariant);

        private readonly GenericParameter parameter;

        public CecilParameter(GenericParameter parameter)
        {
            this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public bool Equals(IParameter other)
        {
            // FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
            return other != null && this.Identifier == other.Identifier;
        }

        public override bool Equals(object obj)
        {
            return obj is CecilParameter other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Parameter={this.Identifier}}}";
        }
    }
}
