using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Reflection;
using NBrowse.Reflection.Mono;

namespace NBrowse
{
	public class Repository : IDisposable
	{
		private readonly ScriptOptions options;
		private readonly CecilProject project;

		public Repository(IEnumerable<string> sources)
		{
			var imports = new[] { "NBrowse.Selection", "System", "System.Collections.Generic", "System.Linq" };
			var references = new[] { typeof(Repository).Assembly };

			this.options = ScriptOptions.Default
				.WithImports(imports)
				.WithReferences(references);
			this.project = new CecilProject(sources);
		}

		public void Dispose()
		{
			this.project.Dispose();
		}

		public async Task<object> Query(string expression)
		{
			var selector = await CSharpScript.EvaluateAsync<Func<IProject, object>>(expression, this.options);

			return selector(this.project);
		}
	}
}