using System;
using System.Collections.Generic;

namespace NBrowse.Reflection.Empty;

internal class EmptyImplementation : Implementation
{
    public override Method Parent { get; }
    public override IEnumerable<Method> ReferencedMethods => Array.Empty<Method>();
    public override IEnumerable<Type> ReferencedTypes => Array.Empty<Type>();

    public EmptyImplementation(Method parent)
    {
        Parent = parent;
    }
}