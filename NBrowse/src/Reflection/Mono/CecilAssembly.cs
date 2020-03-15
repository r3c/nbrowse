using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilAssembly : Assembly
	{
		public override IEnumerable<Attribute> Attributes => this.assembly != null
			? this.assembly.CustomAttributes.Select(attribute => new CecilAttribute(attribute, this.project))
			: Enumerable.Empty<Attribute>();

		public override string Culture => this.name.Culture;

		public override string FileName => this.assembly?.MainModule?.FileName;

		public override string Identifier => this.name.FullName;

		public override string Name => this.name.Name;

		public override IEnumerable<Assembly> References => this.assembly != null
			? this.assembly.MainModule.AssemblyReferences.Select(referenceName =>
			{
				var reference = this.project.FilterAssemblies(new[] {referenceName.Name}).FirstOrDefault();

				if (reference != null && reference.Identifier == referenceName.FullName)
					return reference;

				return new CecilAssembly(referenceName);
			})
			: Enumerable.Empty<Assembly>();

		public override Version Version => this.name.Version;

		public override IEnumerable<Type> Types => this.assembly != null
			? this.assembly.MainModule.GetTypes().Select(type => new CecilType(type, this.project))
			: Array.Empty<CecilType>();

		private readonly AssemblyDefinition assembly;
		private readonly Project project;
		private readonly AssemblyNameReference name;

		public CecilAssembly(AssemblyDefinition assembly, Project project)
		{
			if (assembly?.MainModule == null)
				throw new ArgumentNullException(nameof(assembly));

			this.assembly = assembly;
			this.name = assembly.Name;
			this.project = project;
		}

		private CecilAssembly(AssemblyNameReference name)
		{
			this.assembly = null;
			this.name = name;
			this.project = null;
		}

		public override bool Equals(Assembly other)
		{
			return !object.ReferenceEquals(other, null) && this.Culture == other.Culture && this.Name == other.Name &&
			       this.Version == other.Version;
		}
	}
}
