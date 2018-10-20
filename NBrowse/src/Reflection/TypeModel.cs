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
        public AssemblyModel Assembly => new AssemblyModel(_type.Module.Assembly);
        public IEnumerable<FieldModel> Fields => _type.Fields.Select(field => new FieldModel(field));
        //public IEnumerable<MethodModel> Methods => _type.GetMethods(Discovery.Bindings).Where(m => Discovery.IsVisible(m)).Select(m => new MethodModel(m));
        public string FullName => $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";
        public string Name => _type.Name;
        public string Namespace => _type.Namespace;

        private readonly TypeDefinition _type;

        public TypeModel(TypeDefinition type)
        {
            _type = type;
        }

        public override string ToString()
        {
            return $"{{Type={FullName}}}";
        }
    }
}