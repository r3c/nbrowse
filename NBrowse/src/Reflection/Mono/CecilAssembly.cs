using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Newtonsoft.Json;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAssembly : IAssembly
	{
		public string FileName => this.module?.FileName ?? string.Empty;

		public string Identifier => this.assembly.FullName;

		public string Name => this.assembly.Name.Name;

		[JsonIgnore]
		public IEnumerable<string> References => (this.module?.AssemblyReferences ?? Enumerable.Empty<AssemblyNameReference>()).Select(reference => reference.FullName);

		public Version Version => this.assembly.Name.Version;

		[JsonIgnore]
		public IEnumerable<IType> Types =>
			(this.module?.GetTypes() ?? Array.Empty<TypeDefinition>()).Select(type => new CecilType(type));

		private readonly AssemblyDefinition assembly;
		private readonly ModuleDefinition module;

		public CecilAssembly(AssemblyDefinition assembly)
		{
			this.assembly = assembly;
			this.module = assembly.Modules.FirstOrDefault(module => module.IsMain);
		}

		public bool Equals(IAssembly other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return other != null && this.Identifier == other.Identifier;
		}

		public override bool Equals(object obj)
		{
			return obj is CecilAssembly other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.assembly.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Assembly={this.Identifier}}}";
		}
	}
}