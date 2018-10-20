using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBrowse.Reflection
{
    public struct FieldModel
    {
        public bool IsPrivate => _field.IsPrivate;
        public bool IsPublic => _field.IsPublic;
        public bool IsStatic => _field.IsStatic;
        public string Name => _field.Name;
        public TypeModel Parent => new TypeModel(_field.DeclaringType);
        public TypeModel Type => new TypeModel(_field.FieldType);

        private readonly FieldInfo _field;

        public FieldModel(FieldInfo field)
        {
            _field = field;
        }

        public override string ToString()
        {
            return $"{{Field={Parent.Namespace}.{Parent.Name}.{Name}}}";
        }
    }
}