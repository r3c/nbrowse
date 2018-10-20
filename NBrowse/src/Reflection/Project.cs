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

		public Type FindType(string fullName)
		{
			foreach (var assembly in _assemblies)
			{
				foreach (var type in assembly.Types)
				{
					if (type.FullName == fullName)
						return type;
				}
			}

			throw new ArgumentOutOfRangeException(nameof(fullName), fullName, "no matching type found");
		}

		public Project(IEnumerable<Assembly> assemblies)
		{
			_assemblies = assemblies.ToArray();
		}
	}
}