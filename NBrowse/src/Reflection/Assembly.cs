using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Selection;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public struct Assembly : IEquatable<Assembly>
	{
		public string FileName => _module?.FileName ?? string.Empty;

		public string Identifier => _assembly.FullName;

		public string Name => _assembly.Name.Name;

		[JsonIgnore]
		public IEnumerable<string> References => (_module?.AssemblyReferences ?? Enumerable.Empty<AssemblyNameReference>()).Select(reference => reference.FullName);

		public Version Version => _assembly.Name.Version;

		[JsonIgnore]
		public IEnumerable<Type> Types => (_module?.GetTypes() ?? Array.Empty<TypeDefinition>()).Select(type => new Type(type));

		private readonly AssemblyDefinition _assembly;
		private readonly ModuleDefinition _module;

		public static bool operator ==(Assembly lhs, Assembly rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Assembly lhs, Assembly rhs)
		{
			return !lhs.Equals(rhs);
		}

		public Assembly(AssemblyDefinition assembly)
		{
			_assembly = assembly;
			_module = assembly.Modules.FirstOrDefault(module => module.IsMain);
		}

		public bool Equals(Assembly other)
		{
			return _assembly.MetadataToken.RID == other._assembly.MetadataToken.RID;
		}

		public override bool Equals(object o)
		{
			return o is Assembly other && Equals(other);
		}

		public override int GetHashCode()
		{
			return _assembly.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Assembly={Identifier}}}";
		}
	}
}