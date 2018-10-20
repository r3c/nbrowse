using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
    public struct Type
    {
        public IEnumerable<Field> Fields => _definition != null ? _definition.Fields.Select(field => new Field(field)) : Array.Empty<Field>();
        public string FullName => $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";
        public IEnumerable<Method> Methods => _definition != null ? _definition.Methods.Select(method => new Method(method)) : Array.Empty<Method>();
        public string Name => _reference.Name;
        public string Namespace => _reference.Namespace;
        public Assembly Parent => new Assembly(_reference.Module.Assembly);

        private readonly TypeDefinition _definition;
        private readonly TypeReference _reference;

        public Type(TypeDefinition type)
        {
            _definition = type;
            _reference = type;
        }

        public Type(TypeReference type)
        {
            _definition = type.Resolve();
            _reference = type;
        }

        internal bool Equals(TypeReference type)
        {
            // FIXME: most probably inaccurate
            return _reference.FullName == type.FullName;
        }

        public override string ToString()
        {
            return $"{{Type={FullName}}}";
        }
    }
}