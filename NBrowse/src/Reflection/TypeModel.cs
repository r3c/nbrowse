using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
    public struct TypeModel
    {
        public IEnumerable<FieldModel> Fields => _definition != null ? _definition.Fields.Select(field => new FieldModel(field)) : Array.Empty<FieldModel>();
        public string FullName => $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";
        public IEnumerable<MethodModel> Methods => _definition != null ? _definition.Methods.Select(method => new MethodModel(method)) : Array.Empty<MethodModel>();
        public string Name => _reference.Name;
        public string Namespace => _reference.Namespace;
        public AssemblyModel Parent => new AssemblyModel(_reference.Module.Assembly);

        private readonly TypeDefinition _definition;
        private readonly TypeReference _reference;

        public TypeModel(TypeDefinition type)
        {
            _definition = type;
            _reference = type;
        }

        public TypeModel(TypeReference type)
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