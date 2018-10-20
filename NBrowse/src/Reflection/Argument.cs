using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
    public struct Argument
    {
        public string FullName => $"{Type.FullName} {Name}";
        public string Name => _argument.Name;
        public Type Type => new Type(_argument.ParameterType);

        private readonly ParameterDefinition _argument;

        public Argument(ParameterDefinition argument)
        {
            _argument = argument;
        }

        public override string ToString()
        {
            return $"{{Argument={Name}}}";
        }
    }
}