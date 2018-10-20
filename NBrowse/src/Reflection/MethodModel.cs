using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NBrowse.Reflection;

namespace NBrowse.Reflection
{
	public struct MethodModel
	{
		public IEnumerable<ArgumentModel> Arguments => _method.Parameters.Select(argument => new ArgumentModel(argument));
		public TypeModel Parent => new TypeModel(_method.DeclaringType);
		public string FullName => $"{Parent.FullName}.{Name}({string.Join(", ", Arguments.Select(argument => argument.FullName))})";
		public string Name => _method.Name;

		private readonly MethodDefinition _method;

		public MethodModel(MethodDefinition method)
		{
			_method = method;
		}

		public override string ToString()
		{
			return $"{{Method={FullName}}}";
		}
	}
}