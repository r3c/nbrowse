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
        public string Identifier => $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";
        public IEnumerable<Method> Methods => _definition != null ? _definition.Methods.Select(method => new Method(method)) : Array.Empty<Method>();
        public Model Model => _definition.IsEnum ? Model.Enumeration : (_definition.IsInterface ? Model.Interface : (_definition.IsValueType ? Model.Structure : Model.Class));
        public string Name => _reference.Name;
        public string Namespace => _definition == null ? string.Empty : (_definition.IsNested ? new Type(_definition.DeclaringType).Namespace : _definition.Namespace);
        public IEnumerable<Parameter> Parameters => _definition != null ? _definition.GenericParameters.Select(parameter => new Parameter(parameter)) : Array.Empty<Parameter>();
        public Assembly Parent => new Assembly(_reference.Module.Assembly);

        public Visibility Visibility
        {
            get
            {
                if (_definition == null)
                    return Visibility.Public;

                if (_definition.IsNested)
                    return _definition.IsNestedPublic ? Visibility.Public : (_definition.IsNestedFamily ? Visibility.Protected : (_definition.IsNestedPrivate ? Visibility.Private : Visibility.Internal));

                return _definition.IsPublic ? Visibility.Public : (_definition.IsNotPublic ? Visibility.Private : Visibility.Internal);
            }
        }

        private readonly TypeDefinition _definition;
        private readonly TypeReference _reference;

        public Type(TypeDefinition type)
        {
            _definition = type;
            _reference = type;
        }

        public Type(TypeReference type)
        {
            _definition = type.IsDefinition ? type.Resolve() : null;
            _reference = type;
        }

        public bool Equals(Type other)
        {
            // FIXME: most probably inaccurate, waiting for https://github.com/jbevain/cecil/pull/394
            return _reference.FullName == other._reference.FullName;
        }

        public override string ToString()
        {
            return $"{{Type={Identifier}}}";
        }
    }
}