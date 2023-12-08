using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Reflection.Empty;

namespace NBrowse.Reflection.Mono
{
    internal class CecilMethod : Method
    {
        public override IEnumerable<Argument> Arguments =>
            this.reference.Parameters.Select(argument => new CecilArgument(argument, this.project));

        public override IEnumerable<Attribute> Attributes =>
            this.definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute, this.project)) ??
            Array.Empty<CecilAttribute>();

        public override Binding Binding => this.definition == null
            ? Binding.Unknown
            : (this.definition.IsConstructor
                ? Binding.Constructor
                : (this.definition.IsStatic
                    ? Binding.Static
                    : Binding.Instance));

        public override Implementation Implementation => this.definition?.Body != null
            ? new CecilImplementation(this, this.definition.Body, this.project) as Implementation
            : new EmptyImplementation(this);

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
            this.reference.GenericParameters.Select(parameter => new CecilParameter(parameter, this.project));

        public override Type Parent => new CecilType(this.reference.DeclaringType, this.project);

        public override Type ReturnType => new CecilType(this.reference.ReturnType, this.project);

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
        private readonly Project project;
        private readonly MethodReference reference;

        public CecilMethod(MethodReference reference, Project project)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            if (!(reference is MethodDefinition definition))
            {
                try
                {
                    definition = reference.IsDefinition || reference.Module.AssemblyResolver != null
                        ? reference.Resolve()
                        : null;
                }
                // FIXME: Mono.Cecil throws an exception when trying to resolve a
                // non-loaded assembly and I don't know how I can safely avoid that
                // without catching the exception.
                catch (AssemblyResolutionException)
                {
                    definition = null;
                }
            }

            this.definition = definition;
            this.project = project;
            this.reference = reference;
        }

        public override bool Equals(Method other)
        {
            return !object.ReferenceEquals(other, null) && this.Name == other.Name && this.Parent == other.Parent &&
                   this.ReturnType == other.ReturnType && this.Arguments.SequenceEqual(other.Arguments) &&
                   this.Parameters.SequenceEqual(other.Parameters);
        }
    }
}