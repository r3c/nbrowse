using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
    internal class CecilParameter : IParameter
    {
        public IEnumerable<IType> Constraints =>
            this.parameter.Constraints.Select(constraint => new CecilType(constraint.ConstraintType) as IType);

        public bool HasDefaultConstructor => this.parameter.HasDefaultConstructorConstraint;

        public string Identifier => this.parameter.FullName;

        public string Name => this.parameter.Name;

        public Variance Variance => this.parameter.IsContravariant
            ? Variance.Contravariant
            : (this.parameter.IsCovariant ? Variance.Covariant : Variance.Invariant);

        private readonly GenericParameter parameter;

        public CecilParameter(GenericParameter parameter)
        {
            this.parameter = parameter;
        }

        public override string ToString()
        {
            return $"{{Parameter={this.Identifier}}}";
        }
    }
}