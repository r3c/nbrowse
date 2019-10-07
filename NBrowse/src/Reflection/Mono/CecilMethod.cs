using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilMethod : IMethod
	{
		public IEnumerable<IArgument> Arguments =>
			this.reference.Parameters.Select(argument => new CecilArgument(argument, this.parent));

		public IEnumerable<IAttribute> Attributes =>
			this.definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute, this.parent)) ??
			Array.Empty<CecilAttribute>();

		public Binding Binding => this.definition == null
			? Binding.Unknown
			: (this.definition.IsConstructor
				? Binding.Constructor
				: (this.definition.IsStatic
					? Binding.Static
					: Binding.Instance));

		public IImplementation ImplementationOrNull => this.definition?.Body != null
			? new CecilImplementation(this.definition.Body, this.parent)
			: null;

		public Definition Definition => this.definition == null
			? Definition.Unknown
			: (this.definition.IsAbstract
				? Definition.Abstract
				: (this.definition.IsFinal
					? Definition.Final
					: (this.definition.IsVirtual
						? Definition.Virtual
						: Definition.Concrete)));

		public string Identifier =>
			$"{this.Parent.Identifier}.{this.Name}({string.Join(", ", this.Arguments.Select(argument => argument.Identifier))})";

		public string Name => this.reference.Name;

		public IEnumerable<IParameter> Parameters =>
			this.reference.GenericParameters.Select(parameter => new CecilParameter(parameter, this.parent));

		public IType Parent => new CecilType(this.reference.DeclaringType, this.parent);

		public IType ReturnType => new CecilType(this.reference.ReturnType, this.parent);

		public Visibility Visibility => this.definition == null
			? Visibility.Unknown
			: (this.definition.IsPublic
				? Visibility.Public
				: (this.definition.IsPrivate
					? Visibility.Private
					: (this.definition.IsFamily
						? Visibility.Protected
						: Visibility.Internal)));

		private readonly MethodDefinition definition;
		private readonly IAssembly parent;
		private readonly MethodReference reference;

		public CecilMethod(MethodReference reference, IAssembly parent)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));

			if (!(reference is MethodDefinition definition))
			{
				definition = reference.IsDefinition || reference.Module.AssemblyResolver != null
					? reference.Resolve()
					: null;
			}

			this.definition = definition;
			this.parent = parent;
			this.reference = reference;
		}

		public bool Equals(IMethod other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return other != null && this.Identifier == other.Identifier;
		}

		public override bool Equals(object obj)
		{
			return obj is CecilMethod other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.Identifier.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Method={this.Identifier}}}";
		}
	}
}
