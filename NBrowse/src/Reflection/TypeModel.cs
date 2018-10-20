using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
    public struct TypeModel
    {
        public AssemblyModel Assembly => new AssemblyModel(_type.Assembly);
        public IEnumerable<FieldModel> Fields => _type.GetFields(Discovery.Bindings).Where(f => Discovery.IsVisible(f)).Select(f => new FieldModel(f));
        //public IEnumerable<MethodModel> Methods => _type.GetMethods(Discovery.Bindings).Where(m => Discovery.IsVisible(m)).Select(m => new MethodModel(m));
        public string Name => _type.Name;
        public string Namespace => _type.Namespace;

        private readonly Type _type;

        public TypeModel(Type type)
        {
            _type = type;
        }

        public override string ToString()
        {
            return $"{{Type={Namespace}.{Name}}}";
        }
    }
}