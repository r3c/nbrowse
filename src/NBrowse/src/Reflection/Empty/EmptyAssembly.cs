using System;
using System.Collections.Generic;

namespace NBrowse.Reflection.Empty;

internal class EmptyAssembly : Assembly
{
    public static readonly Assembly Instance = new EmptyAssembly();

    private static readonly Version EmptyVersion = new();

    public override IEnumerable<Attribute> Attributes => Array.Empty<Attribute>();
    public override string Culture => string.Empty;
    public override string FileName => string.Empty;
    public override string Identifier => "<unresolved>";
    public override string Name => "<unresolved>";
    public override IEnumerable<Assembly> References => Array.Empty<Assembly>();
    public override Version Version => EmptyVersion;
    public override IEnumerable<Type> Types => Array.Empty<Type>();

    private EmptyAssembly()
    {
    }

    public override bool Equals(Assembly other)
    {
        return other is EmptyAssembly;
    }
}