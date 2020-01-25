using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilProject : Project, IDisposable
	{
		public override IEnumerable<Assembly> Assemblies => this.assemblies.Values;

		private readonly IReadOnlyDictionary<string, CecilAssembly> assemblies;
		private readonly IReadOnlyList<IDisposable> resources;

		public CecilProject(IEnumerable<string> sources)
		{
			var parameters = new ReaderParameters {AssemblyResolver = new DefaultAssemblyResolver(), InMemory = true};
			var assemblies = sources.Select(source => AssemblyDefinition.ReadAssembly(source, parameters)).ToList();

			this.assemblies = assemblies.Select(assembly => new CecilAssembly(assembly, this))
				.GroupBy(assembly => assembly.Name).ToDictionary(group => group.Key,
					group =>
					{
						if (group.Count() > 1)
						{
							Console.Error.WriteLine(
								$"Warning: project contains more than one assembly named '{group.Key}', only one will be accessible.");
						}

						return group.First();
					});
			this.resources = assemblies;
		}

		public void Dispose()
		{
			foreach (var resource in this.resources)
				resource.Dispose();
		}

		public override IEnumerable<Assembly> FilterAssemblies(IEnumerable<string> names)
		{
			foreach (var name in names)
			{
				if (this.assemblies.TryGetValue(name, out var assembly))
					yield return assembly;
			}
		}

		public override Assembly FindAssembly(string name)
		{
			if (!this.assemblies.TryGetValue(name, out var assembly))
				throw new ArgumentOutOfRangeException(nameof(name), name, "no matching assembly found");

			return assembly;
		}

		public override Method FindMethod(string search)
		{
			var byIdentifier = (Method) null;
			var byIdentifierFound = false;
			var byName = (Method) null;
			var byNameFound = false;
			var byParent = (Method) null;
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

		public override Type FindType(string search)
		{
			var byGeneric = (Type) null;
			var byGenericFound = false;
			var byIdentifier = (Type) null;
			var byIdentifierFound = false;
			var byName = (Type) null;
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
	}
}