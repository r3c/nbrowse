using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public class Project
	{
		[JsonIgnore]
		public IEnumerable<Assembly> Assemblies => _assemblies.Values;

		private readonly IReadOnlyDictionary<string, Assembly> _assemblies;

		public IEnumerable<Assembly> FilterAssemblies(IEnumerable<string> fullNames)
		{
			foreach (var fullName in fullNames)
			{
				if (_assemblies.TryGetValue(fullName, out Assembly assembly))
					yield return assembly;
			}
		}

		public Assembly FindAssembly(string fullName)
		{
			if (!_assemblies.TryGetValue(fullName, out Assembly assembly))
				throw new ArgumentOutOfRangeException(nameof(fullName), fullName, "no matching assembly found");

			return assembly;
		}

		public Method FindMethod(string search)
		{
			var byIdentifier = new Method?();
			var byIdentifierFound = false;
			var byName = new Method?();
			var byNameFound = false;

			foreach (var method in _assemblies.Values.SelectMany(a => a.Types).SelectMany(t => t.Methods))
			{
				if (method.Identifier == search)
				{
					if (byIdentifierFound)
						byIdentifier = null;
					else
						byIdentifier = method;

					byIdentifierFound = true;
				}
				else if (method.Name == search)
				{
					if (byNameFound)
						byName = null;
					else
						byName = method;

					byNameFound = true;
				}
			}

			if (byIdentifierFound)
			{
				if (byIdentifier.HasValue)
					return byIdentifier.Value;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one method match identifier '{search}'");
			}

			if (byNameFound)
			{
				if (byName.HasValue)
					return byName.Value;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one method match name '{search}'");
			}

			throw new ArgumentOutOfRangeException(nameof(search), search, "no matching method found");
		}

		public Type FindType(string search)
		{
			var byIdentifier = new Type?();
			var byIdentifierFound = false;
			var byName = new Type?();
			var byNameFound = false;

			foreach (var type in _assemblies.Values.SelectMany(a => a.Types))
			{
				if (type.Identifier == search)
				{
					if (byIdentifierFound)
						byIdentifier = null;
					else
						byIdentifier = type;

					byIdentifierFound = true;
				}
				else if (type.Name == search)
				{
					if (byNameFound)
						byName = null;
					else
						byName = type;

					byNameFound = true;
				}
			}

			if (byIdentifierFound)
			{
				if (byIdentifier.HasValue)
					return byIdentifier.Value;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one type match identifier '{search}'");
			}

			if (byNameFound)
			{
				if (byName.HasValue)
					return byName.Value;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one type match name '{search}'");
			}

			throw new ArgumentOutOfRangeException(nameof(search), search, "no matching type found");
		}

		public Project(IEnumerable<Assembly> assemblies)
		{
			var byIdentifier = new Dictionary<string, Assembly>();

			foreach (var assembly in assemblies)
				byIdentifier[assembly.Name] = assembly;

			_assemblies = byIdentifier;
		}
	}
}