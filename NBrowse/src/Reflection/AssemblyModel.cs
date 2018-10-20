using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
    public struct AssemblyModel
    {
        public IEnumerable<TypeModel> Types => _assembly.Modules.SelectMany(module => module.GetTypes()).Where(type => Discovery.IsVisible(type)).Select(type => new TypeModel(type));
        public string Name => _assembly.Name.Name;
        public Version Version => _assembly.Name.Version;

        private readonly AssemblyDefinition _assembly;

        public AssemblyModel(AssemblyDefinition assembly)
        {
            _assembly = assembly;
        }

        public override string ToString()
        {
            return $"{{Assembly={Name}}}";
        }
    }
}