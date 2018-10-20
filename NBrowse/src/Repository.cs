using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Reflection;

namespace NBrowse
{
    public class Repository
    {
        private readonly IReadOnlyList<AssemblyModel> _assemblies;
        private readonly ScriptOptions _options;

        public Repository(IEnumerable<string> sources)
        {
            var assemblies = sources.Select(path => Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, path)));
            var imports = new [] { "System", "System.Collections.Generic", "System.Linq" };
            var references = new [] { typeof(Repository).Assembly }.Concat(assemblies);

            _assemblies = assemblies.Select(assembly => new AssemblyModel(assembly)).ToArray();
            _options = ScriptOptions.Default
                .WithImports(imports)
                .WithReferences(references);
        }

        public async Task<object> Query(string expression)
        {
            Func<IReadOnlyList<AssemblyModel>, object> selector = await CSharpScript.EvaluateAsync<Func<IReadOnlyList<AssemblyModel>, object>>(expression, _options);

            return selector(_assemblies);
        }
    }
}