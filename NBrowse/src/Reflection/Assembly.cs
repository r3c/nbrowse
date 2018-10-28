using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public struct Assembly
	{
		// See: https://github.com/jbevain/cecil/wiki/HOWTO
		private static readonly Func<TypeDefinition, bool> IsVisible = member => member.CustomAttributes.All(attribute => attribute.AttributeType.FullName != typeof(CompilerGeneratedAttribute).FullName);

		public string FileName => _module?.FileName ?? string.Empty;
		public string Identifier => _assembly.FullName;
		public string Name => _assembly.Name.Name;
		[JsonIgnore]
		public IEnumerable<string> References => (_module?.AssemblyReferences ?? Array.Empty<AssemblyNameReference>() as ICollection<AssemblyNameReference>).Select(reference => reference.FullName);
		public Version Version => _assembly.Name.Version;
		[JsonIgnore]
		public IEnumerable<Type> Types => (_module?.GetTypes() ?? Array.Empty<TypeDefinition>()).Where(IsVisible).Select(type => new Type(type));

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