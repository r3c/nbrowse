using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace NBrowse.Reflection
{
    public struct Type : IEquatable<Type>
    {
        public IEnumerable<Attribute> Attributes => _definition != null ? _definition.CustomAttributes.Select(attribute => new Attribute(attribute)) : Array.Empty<Attribute>();
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

        public bool Equals(Type other)
        {
            // FIXME: most probably inaccurate, waiting for https://github.com/jbevain/cecil/pull/394
            return _reference.FullName == other._reference.FullName;
        }

        public override string ToString()
        {
            return $"{{Type={FullName}}}";
        }
    }
}