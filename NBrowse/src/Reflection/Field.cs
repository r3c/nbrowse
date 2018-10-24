using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace NBrowse.Reflection
{
    public struct Field
    {
        public Binding Binding => _field.IsStatic ? Binding.Static : Binding.Instance;
        public string Identifier => $"{Parent.Identifier}.{Name}";
        public string Name => _field.Name;
        public Type Parent => new Type(_field.DeclaringType);
        public Type Type => new Type(_field.FieldType);
        public Visibility Visibility => _field.IsPublic ? Visibility.Public : (_field.IsPrivate ? Visibility.Private : (_field.IsFamily ? Visibility.Protected : Visibility.Internal));

        private readonly FieldDefinition _field;

        public Field(FieldDefinition field)
        {
            _field = field;
        }

        public override string ToString()
        {
            return $"{{Field={Identifier}}}";
        }
    }
}