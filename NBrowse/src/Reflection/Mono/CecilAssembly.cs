using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAssembly : IAssembly
	{
		public string FileName => this.module?.FileName ?? string.Empty;

		public string Identifier => this.module.Assembly.FullName;

		public string Name => this.module.Assembly.Name.Name;

		public IEnumerable<string> References =>
			this.module?.AssemblyReferences?.Select(reference => reference.FullName) ?? Enumerable.Empty<string>();

		public Version Version => this.module.Assembly.Name.Version;

		public IEnumerable<IType> Types =>
			this.module?.GetTypes()?.Select(type => new CecilType(type)) ?? Array.Empty<CecilType>();

		private readonly ModuleDefinition module;

		public CecilAssembly(ModuleDefinition module)
		{
			this.module = module;
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
			return this.module.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Assembly={this.Identifier}}}";
		}
	}
}