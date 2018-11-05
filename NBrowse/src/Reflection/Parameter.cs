using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
    public struct Parameter
    {
        [JsonIgnore]
        public IEnumerable<Type> Constraints => _parameter.Constraints.Select(constraint => new Type(constraint));

        public bool HasDefaultConstructor => _parameter.HasDefaultConstructorConstraint;

        public string Identifier => _parameter.FullName;

        public bool IsContravariant => _parameter.IsContravariant;

        public bool IsCovariant => _parameter.IsCovariant;

        public string Name => _parameter.Name;

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