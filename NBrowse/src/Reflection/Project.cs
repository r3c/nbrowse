using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Reflection.Mono;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public class Project : IDisposable
	{
		[JsonIgnore]
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
			}

			if (byIdentifierFound)
			{
				if (byIdentifier != null)
					return byIdentifier;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one method match identifier '{search}'");
			}

			if (byNameFound)
			{
				if (byName != null)
					return byName;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one method match name '{search}'");
			}

			throw new ArgumentOutOfRangeException(nameof(search), search, "no matching method found");
		}

		public IType FindType(string search)
		{
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
			}

			if (byIdentifierFound)
			{
				if (byIdentifier != null)
					return byIdentifier;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one type match identifier '{search}'");
			}

			if (byNameFound)
			{
				if (byName != null)
					return byName;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one type match name '{search}'");
			}

			throw new ArgumentOutOfRangeException(nameof(search), search, "no matching type found");
		}

		public Project(IEnumerable<string> sources)
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