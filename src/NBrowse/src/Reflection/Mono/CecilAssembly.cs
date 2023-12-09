using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilAssembly : Assembly
{
    public override IEnumerable<Attribute> Attributes => _assembly != null
        ? _assembly.CustomAttributes.Select(attribute => new CecilAttribute(attribute, _project))
        : Enumerable.Empty<Attribute>();

    public override string Culture => _name.Culture;

    public override string FileName => _assembly?.MainModule?.FileName;

    public override string Identifier => _name.FullName;

    public override string Name => _name.Name;

    public override IEnumerable<Assembly> References => _assembly != null
        ? _assembly.MainModule.AssemblyReferences.Select(referenceName =>
        {
            var reference = _project.FilterAssemblies(new[] { referenceName.Name }).FirstOrDefault();

            if (reference != null && reference.Identifier == referenceName.FullName)
                return reference;

            return new CecilAssembly(referenceName);
        })
        : Enumerable.Empty<Assembly>();

    public override Version Version => _name.Version;

    public override IEnumerable<Type> Types => _assembly != null
        ? _assembly.MainModule.GetTypes().Select(type => new CecilType(type, _project))
        : Array.Empty<CecilType>();

    private readonly AssemblyDefinition _assembly;
    private readonly Project _project;
    private readonly AssemblyNameReference _name;

    public CecilAssembly(AssemblyDefinition assembly, Project project)
    {
        if (assembly?.MainModule == null)
            throw new ArgumentNullException(nameof(assembly));

        _assembly = assembly;
        _name = assembly.Name;
        _project = project;
    }

    private CecilAssembly(AssemblyNameReference name)
    {
        _assembly = null;
        _name = name;
        _project = null;
    }

    public override bool Equals(Assembly other)
    {
        return !ReferenceEquals(other, null) && Culture == other.Culture && Name == other.Name &&
               Version == other.Version;
    }
}