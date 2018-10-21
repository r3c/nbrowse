using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace NBrowse.Reflection
{
    public struct Parameter
    {
        public bool HasDefaultConstructorConstraint => _parameter.HasDefaultConstructorConstraint;
        public bool HasValueTypeConstraint => _parameter.HasNotNullableValueTypeConstraint;
        public string Identifier => _parameter.FullName;
        public bool IsContravariant => _parameter.IsContravariant;
        public bool IsCovariant => _parameter.IsCovariant;
        public string Name => _parameter.Name;
        public Type Type => new Type(_parameter);

        private readonly GenericParameter _parameter;

        public Parameter(GenericParameter parameter)
        {
            _parameter = parameter;
        }

        public override string ToString()
        {
            return $"{{Parameter={Identifier}}}";
        }
    }
}