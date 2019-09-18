using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAssembly : IAssembly
	{
		public IEnumerable<IAttribute> Attributes =>
			this.assembly.CustomAttributes.Select(attribute => new CecilAttribute(attribute));

		public string FileName => this.assembly.MainModule.FileName;

		public string Identifier => this.assembly.FullName;

		public string Name => this.assembly.Name.Name;

		public IEnumerable<string> References =>
			this.assembly.MainModule.AssemblyReferences?.Select(reference => reference.FullName) ??
			Enumerable.Empty<string>();

		public Version Version => this.assembly.Name.Version;

		public IEnumerable<IType> Types =>
			this.assembly.MainModule?.GetTypes()?.Select(type => new CecilType(type)) ?? Array.Empty<CecilType>();

		private readonly AssemblyDefinition assembly;

		public CecilAssembly(AssemblyDefinition assembly)
		{
			if (assembly?.MainModule == null)
				throw new ArgumentNullException(nameof(assembly));

			this.assembly = assembly;
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
			return this.Identifier.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Assembly={this.Identifier}}}";
		}
	}
}
