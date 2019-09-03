using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NBrowse.Reflection;

namespace NBrowse
{
	public class Repository : IDisposable
	{
		private readonly ScriptOptions options;
		private readonly Project project;

		public Repository(IEnumerable<string> sources)
		{
			var imports = new[] { "NBrowse.Selection", "System", "System.Collections.Generic", "System.Linq" };
			var references = new[] { typeof(Repository).Assembly };

			this.options = ScriptOptions.Default
				.WithImports(imports)
				.WithReferences(references);
			this.project = new Project(sources);
		}

		public void Dispose()
		{
			this.project.Dispose();
		}

		public async Task<object> Query(string expression)
		{
			var selector = await CSharpScript.EvaluateAsync<Func<Project, object>>(expression, this.options);

			return selector(this.project);
		}
	}
}