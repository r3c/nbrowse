using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse
{
	public class Repository : IDisposable
	{
		private readonly IReadOnlyList<AssemblyDefinition> definitions;
		private readonly ScriptOptions options;
		private readonly Project project;

		public Repository(IEnumerable<string> sources)
		{
			var definitions = sources.Select(AssemblyDefinition.ReadAssembly).ToArray();
			var imports = new[] { "NBrowse.Selection", "System", "System.Collections.Generic", "System.Linq" };
			var references = new[] { typeof(Repository).Assembly };

			this.definitions = definitions;
			this.options = ScriptOptions.Default
				.WithImports(imports)
				.WithReferences(references);
			this.project = new Project(definitions.Select(assembly => new Assembly(assembly)));
		}

		public void Dispose()
		{
			foreach (var definition in this.definitions)
				definition.Dispose();
		}

		public async Task<object> Query(string expression)
		{
			var selector = await CSharpScript.EvaluateAsync<Func<Project, object>>(expression, this.options);

			return selector(this.project);
		}
	}
}