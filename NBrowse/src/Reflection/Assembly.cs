using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Selection;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public struct Assembly
	{
		public string FileName => _module?.FileName ?? string.Empty;

		public string Identifier => _assembly.FullName;

		public string Name => _assembly.Name.Name;

		[JsonIgnore]
		public IEnumerable<string> References => (_module?.AssemblyReferences ?? Array.Empty<AssemblyNameReference>() as ICollection<AssemblyNameReference>).Select(reference => reference.FullName);

		public Version Version => _assembly.Name.Version;

		[JsonIgnore]
		public IEnumerable<Type> Types => (_module?.GetTypes() ?? Array.Empty<TypeDefinition>()).Select(type => new Type(type));

		private readonly AssemblyDefinition _assembly;
		private readonly ModuleDefinition _module;

		public Assembly(AssemblyDefinition assembly)
		{
			_assembly = assembly;
			_module = assembly.Modules.FirstOrDefault(module => module.IsMain);
		}

		public override string ToString()
		{
			return $"{{Assembly={Identifier}}}";
		}
	}
}