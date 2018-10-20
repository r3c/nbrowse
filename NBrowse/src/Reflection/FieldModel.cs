using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;

namespace NBrowse.Reflection
{
    public struct FieldModel
    {
        public bool IsPrivate => _field.IsPrivate;
        public bool IsPublic => _field.IsPublic;
        public bool IsStatic => _field.IsStatic;
        public string Name => _field.Name;
        public TypeModel Parent => new TypeModel(_field.DeclaringType);
        public TypeModel Type => new TypeModel(_field.FieldType.Resolve());

        private readonly FieldDefinition _field;

        public FieldModel(FieldDefinition field)
        {
            _field = field;
        }

        public override string ToString()
        {
            return $"{{Field={Parent.FullName}.{Name}}}";
        }
    }
}