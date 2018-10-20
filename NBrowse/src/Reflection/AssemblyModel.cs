using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
    public struct AssemblyModel
    {
        public IEnumerable<TypeModel> Types => _assembly.GetTypes().Where(t => Discovery.IsVisible(t)).Select(t => new TypeModel(t));
        public string Name => _assembly.GetName().Name;
        public Version Version => _assembly.GetName().Version;

        private readonly Assembly _assembly;

        public AssemblyModel(Assembly assembly)
        {
            _assembly = assembly;
        }

        public override string ToString()
        {
            return $"{{Assembly={Name}}}";
        }
    }
}