using System;
using System.Collections.Generic;

namespace NBrowse.Reflection.Empty;

internal class EmptyNImplementation : NImplementation
{
    public override NMethod Parent { get; }
    public override IEnumerable<NMethod> ReferencedMethods => Array.Empty<NMethod>();
    public override IEnumerable<NType> ReferencedTypes => Array.Empty<NType>();

    public EmptyNImplementation(NMethod parent)
    {
        Parent = parent;
    }
}