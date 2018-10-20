using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace NBrowse.Reflection
{
    public struct Attribute
    {
        public string Identifier => Type.Identifier;
        public Type Type => new Type(_attribute.AttributeType);

        private readonly CustomAttribute _attribute;

        public Attribute(CustomAttribute attribute)
        {
            _attribute = attribute;
        }

        public override string ToString()
        {
            return $"{{Attribute={Identifier}}}";
        }
    }
}