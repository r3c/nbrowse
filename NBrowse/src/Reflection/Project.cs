using System;
using System.Collections.Generic;
using System.Linq;

namespace NBrowse.Reflection
{
	public class Project
	{
		public IEnumerable<Assembly> Assemblies => _assemblies;

		private readonly IReadOnlyList<Assembly> _assemblies;

		public Assembly FindAssembly(string fullName)
		{
			foreach (var assembly in _assemblies)
			{
				if (assembly.Name == fullName)
					return assembly;
			}

			throw new ArgumentOutOfRangeException(nameof(fullName), fullName, "no matching assembly found");
		}

		public Type FindType(string search)
		{
			Type? byIdentifier = null;
			Type? byName = null;
			bool foundIdentifier = false;
			bool foundName = false;

			foreach (var assembly in _assemblies)
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

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one type matches identifier '{search}'");
			}

			if (foundName)
			{
				if (byName.HasValue)
					return byName.Value;

				throw new ArgumentOutOfRangeException(nameof(search), search, $"more than one type matches name '{search}'");
			}

			throw new ArgumentOutOfRangeException(nameof(search), search, "no matching type found");
		}

		public Project(IEnumerable<Assembly> assemblies)
		{
			_assemblies = assemblies.ToArray();
		}
	}
}