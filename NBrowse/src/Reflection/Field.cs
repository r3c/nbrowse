using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;

namespace NBrowse.Reflection
{
    public struct Field
    {
        public bool IsPrivate => _field.IsPrivate;
        public bool IsPublic => _field.IsPublic;
        public bool IsStatic => _field.IsStatic;
        public string Name => _field.Name;
        public Type Parent => new Type(_field.DeclaringType);
        public Type Type => new Type(_field.FieldType);

        private readonly FieldDefinition _field;

        public Field(FieldDefinition field)
        {
            _field = field;
        }

        public override string ToString()
        {
            return $"{{Field={Parent.FullName}.{Name}}}";
        }
    }
}