using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilProject : IDisposable, IProject
	{
		public IEnumerable<IAssembly> Assemblies => this.assemblies.Values;

		private readonly IReadOnlyDictionary<string, IAssembly> assemblies;

		private readonly IReadOnlyList<AssemblyDefinition> resources;

		public void Dispose()
		{
			foreach (var resource in this.resources)
				resource.Dispose();
		}

		public IEnumerable<IAssembly> FilterAssemblies(IEnumerable<string> fullNames)
		{
			foreach (var fullName in fullNames)
			{
				if (this.assemblies.TryGetValue(fullName, out var assembly))
					yield return assembly;
			}
		}

		public IAssembly FindAssembly(string fullName)
		{
			if (!this.assemblies.TryGetValue(fullName, out var assembly))
				throw new ArgumentOutOfRangeException(nameof(fullName), fullName, "no matching assembly found");

			return assembly;
		}

		public IMethod FindMethod(string search)
		{
			var byIdentifier = (IMethod) null;
			var byIdentifierFound = false;
			var byName = (IMethod) null;
			var byNameFound = false;
			var byParent = (IMethod) null;
			var byParentFound = false;

			foreach (var method in this.assemblies.Values.SelectMany(a => a.Types).SelectMany(t => t.Methods))
			{
				if (method.Identifier == search)
				{
					byIdentifier = byIdentifierFound ? null : method;
					byIdentifierFound = true;
				}
				else if (method.Name == search)
				{
					byName = byNameFound ? null : method;
					byNameFound = true;
				}
				else if ($"{method.Parent.Name}.{method.Name}" == search)
				{
					byParent = byParentFound ? null : method;
					byParentFound = true;
				}
			}

			if (byIdentifierFound)
			{
				if (byIdentifier != null)
					return byIdentifier;

				throw new AmbiguousMatchException($"more than one method match identifier '{search}'");
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

		public IType FindType(string search)
		{
			var byGeneric = (IType) null;
			var byGenericFound = false;
			var byIdentifier = (IType) null;
			var byIdentifierFound = false;
			var byName = (IType) null;
			var byNameFound = false;

			foreach (var type in this.assemblies.Values.SelectMany(a => a.Types))
			{
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

		public CecilProject(IEnumerable<string> sources)
		{
			var resources = sources.Select(AssemblyDefinition.ReadAssembly).ToArray();
			var byIdentifier = new Dictionary<string, IAssembly>();

			foreach (var assembly in resources.Select(resource => new CecilAssembly(resource)))
				byIdentifier[assembly.Name] = assembly;

			this.assemblies = byIdentifier;
			this.resources = resources;
		}
	}
}