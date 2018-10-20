using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
    public struct ArgumentModel
    {
        public string FullName => $"{Type.FullName} {Name}";
        public string Name => _argument.Name;
        public TypeModel Type => new TypeModel(_argument.ParameterType);

        private readonly ParameterDefinition _argument;

        public ArgumentModel(ParameterDefinition argument)
        {
            _argument = argument;
        }

        public override string ToString()
        {
            return $"{{Argument={Name}}}";
        }
    }
}