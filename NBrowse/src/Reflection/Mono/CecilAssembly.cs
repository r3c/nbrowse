using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAssembly : IAssembly
	{
		public IEnumerable<IAttribute> Attributes =>
			this.assembly?.CustomAttributes?.Select(attribute => new CecilAttribute(attribute, this)) ??
			Enumerable.Empty<IAttribute>();

		public string Culture => this.name.Culture;

		public string FileName => this.assembly?.MainModule?.FileName;

		public string Identifier => this.name.FullName;

		public string Name => this.name.Name;

		public IEnumerable<IAssembly> References =>
			this.assembly?.MainModule?.AssemblyReferences?.Select(referenceName =>
			{
				var reference = this.project.FilterAssemblies(new[] {referenceName.Name}).FirstOrDefault();

				if (reference != null && reference.Identifier == referenceName.FullName)
					return reference;

				return new CecilAssembly(referenceName, this.project);
			}) ??
			Enumerable.Empty<IAssembly>();

		public Version Version => this.name.Version;

		public IEnumerable<IType> Types =>
			this.assembly?.MainModule?.GetTypes()?.Select(type => new CecilType(type, this)) ??
			Array.Empty<CecilType>();

		private readonly AssemblyDefinition assembly;
		private readonly IProject project;
		private readonly AssemblyNameReference name;

		public CecilAssembly(AssemblyDefinition assembly, IProject project)
		{
			if (assembly?.MainModule == null)
				throw new ArgumentNullException(nameof(assembly));

			this.assembly = assembly;
			this.name = assembly.Name;
			this.project = project;
		}

		private CecilAssembly(AssemblyNameReference name, IProject project)
		{
			this.assembly = null;
			this.name = name;
			this.project = project;
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
