using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilNProject : NProject, IDisposable
{
    public override IEnumerable<NAssembly> Assemblies => _assemblies.Values;

    private readonly IReadOnlyDictionary<string, CecilNAssembly> _assemblies;
    private readonly IReadOnlyList<IDisposable> _resources;

    public CecilNProject(IEnumerable<string> sources)
    {
        var parameters = new ReaderParameters { AssemblyResolver = new DefaultAssemblyResolver(), InMemory = true };
        var assemblies = sources.Select(source => AssemblyDefinition.ReadAssembly(source, parameters)).ToList();

        _assemblies = assemblies.Select(assembly => new CecilNAssembly(assembly, this))
            .GroupBy(assembly => assembly.Name).ToDictionary(group => group.Key,
                group =>
                {
                    if (group.Count() > 1)
                        Console.Error.WriteLine(
                            $"Warning: project contains more than one assembly named '{group.Key}', only one will be accessible.");

                    return group.First();
                });
        _resources = assemblies;
    }

    public void Dispose()
    {
        foreach (var resource in _resources)
            resource.Dispose();
    }

    public override IEnumerable<NAssembly> FilterAssemblies(IEnumerable<string> names)
    {
        foreach (var name in names)
            if (_assemblies.TryGetValue(name, out var assembly))
                yield return assembly;
    }

    public override NAssembly FindAssembly(string name)
    {
        if (!_assemblies.TryGetValue(name, out var assembly))
            throw new ArgumentOutOfRangeException(nameof(name), name, "no matching assembly found");

        return assembly;
    }

    public override NMethod FindMethod(string search)
    {
        var byFullName = (NMethod)null;
        var byFullNameFound = false;
        var byIdentifier = (NMethod)null;
        var byIdentifierFound = false;
        var byName = (NMethod)null;
        var byNameFound = false;
        var byParent = (NMethod)null;
        var byParentFound = false;

        foreach (var method in _assemblies.Values.SelectMany(a => a.Types).SelectMany(t => t.Methods))
            if (method.Identifier == search)
            {
                byIdentifier = byIdentifierFound ? null : method;
                byIdentifierFound = true;
            }
            else if ($"{method.Parent.Identifier}.{method.Name}" == search)
            {
                byFullName = byFullNameFound ? null : method;
                byFullNameFound = true;
            }
            else if ($"{method.Parent.Name}.{method.Name}" == search)
            {
                byParent = byParentFound ? null : method;
                byParentFound = true;
            }
            else if (method.Name == search)
            {
                byName = byNameFound ? null : method;
                byNameFound = true;
            }

        if (byIdentifierFound)
        {
            if (byIdentifier != null)
                return byIdentifier;

            throw new AmbiguousMatchException($"more than one method match identifier '{search}'");
        }

        if (byFullNameFound)
        {
            if (byFullName != null)
                return byFullName;

            throw new AmbiguousMatchException($"more than one method match full name '{search}'");
        }

        if (byParentFound)
        {
            if (byParent != null)
                return byParent;

            throw new AmbiguousMatchException($"more than one method match parent+name '{search}'");
        }

        if (byNameFound)
        {
            if (byName != null)
                return byName;

            throw new AmbiguousMatchException($"more than one method match name '{search}'");
        }

        throw new ArgumentOutOfRangeException(nameof(search), search, "no matching method found");
    }

    public override NType FindType(string search)
    {
        var byGeneric = (NType)null;
        var byGenericFound = false;
        var byIdentifier = (NType)null;
        var byIdentifierFound = false;
        var byName = (NType)null;
        var byNameFound = false;

        foreach (var type in _assemblies.Values.SelectMany(a => a.Types))
            if (type.Identifier == search)
            {
                byIdentifier = byIdentifierFound ? null : type;
                byIdentifierFound = true;
            }
            else if (type.Name == search)
            {
                byName = byNameFound ? null : type;
                byNameFound = true;
            }
            else if (Regex.Replace(type.Name, "`[0-9]+$", string.Empty) == search)
            {
                byGeneric = byGenericFound ? null : type;
                byGenericFound = true;
            }

        if (byIdentifierFound)
        {
            if (byIdentifier != null)
                return byIdentifier;

            throw new AmbiguousMatchException($"more than one type match identifier '{search}'");
        }

        if (byNameFound)
        {
            if (byName != null)
                return byName;

            throw new AmbiguousMatchException($"more than one type match name '{search}'");
        }

        if (byGenericFound)
        {
            if (byGeneric != null)
                return byGeneric;

            throw new AmbiguousMatchException($"more than one type match generic name '{search}'");
        }

        throw new ArgumentOutOfRangeException(nameof(search), search, "no matching type found");
    }
}