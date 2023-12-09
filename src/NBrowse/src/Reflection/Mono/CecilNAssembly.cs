using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilNAssembly : NAssembly
{
    public override IEnumerable<NAttribute> Attributes => _assembly != null
        ? _assembly.CustomAttributes.Select(attribute => new CecilNAttribute(attribute, _nProject))
        : Enumerable.Empty<NAttribute>();

    public override string Culture => _name.Culture;

    public override string FileName => _assembly?.MainModule?.FileName;

    public override string Identifier => _name.FullName;

    public override string Name => _name.Name;

    public override IEnumerable<NAssembly> References => _assembly != null
        ? _assembly.MainModule.AssemblyReferences.Select(referenceName =>
        {
            var reference = _nProject.FilterAssemblies(new[] { referenceName.Name }).FirstOrDefault();

            if (reference != null && reference.Identifier == referenceName.FullName)
                return reference;

            return new CecilNAssembly(referenceName);
        })
        : Enumerable.Empty<NAssembly>();

    public override Version Version => _name.Version;

    public override IEnumerable<NType> Types => _assembly != null
        ? _assembly.MainModule.GetTypes().Select(type => new CecilNType(type, _nProject))
        : Array.Empty<CecilNType>();

    private readonly AssemblyDefinition _assembly;
    private readonly NProject _nProject;
    private readonly AssemblyNameReference _name;

    public CecilNAssembly(AssemblyDefinition assembly, NProject nProject)
    {
        if (assembly?.MainModule == null)
            throw new ArgumentNullException(nameof(assembly));

        _assembly = assembly;
        _name = assembly.Name;
        _nProject = nProject;
    }

    private CecilNAssembly(AssemblyNameReference name)
    {
        _assembly = null;
        _name = name;
        _nProject = null;
    }

    public override bool Equals(NAssembly other)
    {
        return !ReferenceEquals(other, null) && Culture == other.Culture && Name == other.Name &&
               Version == other.Version;
    }
}