using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse
{
    public class Repository : IDisposable
    {
        private readonly IReadOnlyList<AssemblyDefinition> _definitions;
        private readonly IReadOnlyList<AssemblyModel> _models;
        private readonly ScriptOptions _options;

        public Repository(IEnumerable<string> sources)
        {
            var definitions = sources.Select(path => AssemblyDefinition.ReadAssembly(path)).ToArray();
            var imports = new [] { "System", "System.Collections.Generic", "System.Linq" };
            var references = new [] { typeof(Repository).Assembly };

            _definitions = definitions;
            _models = definitions.Select(assembly => new AssemblyModel(assembly)).ToArray();
            _options = ScriptOptions.Default
                .WithImports(imports)
                .WithReferences(references);
        }

        public void Dispose()
        {
            foreach (var definition in _definitions)
                definition.Dispose();
        }

        public async Task<object> Query(string expression)
        {
            Func<IReadOnlyList<AssemblyModel>, object> selector = await CSharpScript.EvaluateAsync<Func<IReadOnlyList<AssemblyModel>, object>>(expression, _options);

            return selector(_models);
        }
    }
}