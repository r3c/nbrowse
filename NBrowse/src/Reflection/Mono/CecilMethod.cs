using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilMethod : Method
	{
		public override IEnumerable<Argument> Arguments =>
			this.reference.Parameters.Select(argument => new CecilArgument(argument, this.parent));

		public override IEnumerable<Attribute> Attributes =>
			this.definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute, this.parent)) ??
			Array.Empty<CecilAttribute>();

		public override Binding Binding => this.definition == null
			? Binding.Unknown
			: (this.definition.IsConstructor
				? Binding.Constructor
				: (this.definition.IsStatic
					? Binding.Static
					: Binding.Instance));

		public override Implementation ImplementationOrNull => this.definition?.Body != null
			? new CecilImplementation(this.definition.Body, this.parent)
			: null;

		public override Definition Definition => this.definition == null
			? Definition.Unknown
			: (this.definition.IsAbstract
				? Definition.Abstract
				: (this.definition.IsFinal
					? Definition.Final
					: (this.definition.IsVirtual
						? Definition.Virtual
						: Definition.Concrete)));

		public override string Identifier =>
			$"{this.Parent.Identifier}.{this.Name}({string.Join(", ", this.Arguments.Select(argument => argument.Identifier))})";

		public override string Name => this.reference.Name;

		public override IEnumerable<Parameter> Parameters =>
			this.reference.GenericParameters.Select(parameter => new CecilParameter(parameter, this.parent));

		public override Type Parent => new CecilType(this.reference.DeclaringType, this.parent);

		public override Type ReturnType => new CecilType(this.reference.ReturnType, this.parent);

		public override Visibility Visibility => this.definition == null
			? Visibility.Unknown
			: (this.definition.IsPublic
				? Visibility.Public
				: (this.definition.IsPrivate
					? Visibility.Private
					: (this.definition.IsFamily
						? Visibility.Protected
						: Visibility.Internal)));

		private readonly MethodDefinition definition;
		private readonly Assembly parent;
		private readonly MethodReference reference;

		public CecilMethod(MethodReference reference, Assembly parent)
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

		public override bool Equals(Method other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return !object.ReferenceEquals(other, null) && this.Identifier == other.Identifier;
		}
	}
}
