using System;
using System.Collections.Generic;
using System.Linq;

namespace NBrowse.Reflection
{
	public class Project
	{
		public IEnumerable<AssemblyModel> Assemblies => _assemblies;

		private readonly IReadOnlyList<AssemblyModel> _assemblies;

		public TypeModel FindType(string fullName)
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

		public Project(IEnumerable<AssemblyModel> assemblies)
		{
			_assemblies = assemblies.ToArray();
		}
	}
}