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

		public Type FindType(string search)
		{
			Type? byIdentifier = null;
			Type? byName = null;
			bool foundIdentifier = false;
			bool foundName = false;

			foreach (var assembly in _assemblies.Values)
			{
				foreach (var type in assembly.Types)
				{
					if (type.Identifier == search)
					{
						if (foundIdentifier)
							byIdentifier = null;
						else
							byIdentifier = type;

						foundIdentifier = true;
					}
					else if (type.Name == search)
					{
						if (foundName)
							byName = null;
						else
							byName = type;

						foundName = true;
					}
				}
			}

			if (foundIdentifier)
			{
				if (byIdentifier.HasValue)
					return byIdentifier.Value;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one type match identifier '{search}'");
			}

			if (foundName)
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