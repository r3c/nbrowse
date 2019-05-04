using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
    public struct Parameter
    {
        [JsonIgnore]
        public IEnumerable<Type> Constraints => this.parameter.Constraints.Select(constraint => new Type(constraint));

        public bool HasDefaultConstructor => this.parameter.HasDefaultConstructorConstraint;

        public string Identifier => this.parameter.FullName;

        public bool IsContravariant => this.parameter.IsContravariant;

        public bool IsCovariant => this.parameter.IsCovariant;

        public string Name => this.parameter.Name;

        private readonly GenericParameter parameter;

        public Parameter(GenericParameter parameter)
        {
            this.parameter = parameter;
        }

        public override string ToString()
        {
            return $"{{Parameter={this.Identifier}}}";
        }
    }
}