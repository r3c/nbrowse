using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
    internal class CecilParameter : Parameter
    {
        public override IEnumerable<Type> Constraints =>
            this.parameter.Constraints.Select(constraint => new CecilType(constraint.ConstraintType, this.parent));

        public override bool HasDefaultConstructor => this.parameter.HasDefaultConstructorConstraint;

        public override string Identifier => this.parameter.FullName + (this.parameter.Constraints.Count > 0
                                                 ? " : " + string.Join(", ", this.Constraints)
                                                 : string.Empty);

        public override string Name => this.parameter.Name;

        public override Variance Variance => this.parameter.IsContravariant
            ? Variance.Contravariant
            : (this.parameter.IsCovariant ? Variance.Covariant : Variance.Invariant);

        private readonly GenericParameter parameter;
        private readonly Assembly parent;

        public CecilParameter(GenericParameter parameter, Assembly parent)
        {
            this.parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            this.parent = parent;
        }

		public override bool Equals(Parameter other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return !object.ReferenceEquals(other, null) && this.Identifier == other.Identifier;
		}
    }
}
