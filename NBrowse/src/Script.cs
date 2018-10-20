using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Reflection;

namespace NBrowse
{
    public class Script
    {
        private readonly ScriptOptions _options;

        public Script(IEnumerable<Assembly> assemblies)
        {
            var imports = new [] { "System", "System.Collections.Generic", "System.Linq" };
            var references = new [] { typeof(Script).Assembly }.Concat(assemblies);

            _options = ScriptOptions.Default
                .WithImports(imports)
                .WithReferences(references);
        }

        public async Task<object> Execute<T>(T input, string expression)
        {
            Func<T, object> selector = await CSharpScript.EvaluateAsync<Func<T, object>>(expression, _options);

            return selector(input);
        }
    }
}