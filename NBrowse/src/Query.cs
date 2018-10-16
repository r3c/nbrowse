using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Model;

namespace NBrowse
{
    public class Query
    {
        public delegate object Selector<T>(IEnumerable<T> sources);

        private readonly ScriptOptions _options;

        public Query(IEnumerable<Assembly> assemblies)
        {
            _options = ScriptOptions.Default
                .WithImports("System", "System.Collections.Generic", "System.Linq")
                .WithReferences(assemblies);
        }

        public async Task<IEnumerable<string>> Execute<T>(IEnumerable<T> sources, string expression)
        {
            Selector<T> selector = await CSharpScript.EvaluateAsync<Selector<T>>(expression, _options);
            object untyped = selector(sources);

            if (!(untyped is IEnumerable results))
                throw new InvalidCastException("expression must return an IEnumerable but was " + untyped.GetType());

            return results.Cast<object>().Select(r => r.ToString());
        }
    }
}