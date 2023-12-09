using System;
using System.Collections.Generic;

namespace NBrowse.Reflection.Empty;

internal class EmptyNAssembly : NAssembly
{
    public static readonly NAssembly Instance = new EmptyNAssembly();

    private static readonly Version EmptyVersion = new();

    public override IEnumerable<NAttribute> Attributes => Array.Empty<NAttribute>();
    public override string Culture => string.Empty;
    public override string FileName => string.Empty;
    public override string Identifier => "<unresolved>";
    public override string Name => "<unresolved>";
    public override IEnumerable<NAssembly> References => Array.Empty<NAssembly>();
    public override Version Version => EmptyVersion;
    public override IEnumerable<NType> Types => Array.Empty<NType>();

    private EmptyNAssembly()
    {
    }

    public override bool Equals(NAssembly other)
    {
        return other is EmptyNAssembly;
    }
}