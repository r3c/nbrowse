using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Reflection.Empty;

namespace NBrowse.Reflection.Mono;

internal class CecilMethod : Method
{
    public override IEnumerable<Argument> Arguments =>
        _reference.Parameters.Select(argument => new CecilArgument(argument, _project));

    public override IEnumerable<Attribute> Attributes =>
        _definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute, _project)) ??
        Array.Empty<CecilAttribute>();

    public override Binding Binding => _definition == null
        ? Binding.Unknown
        : _definition.IsConstructor
            ? Binding.Constructor
            : _definition.IsStatic
                ? Binding.Static
                : Binding.Instance;

    public override Implementation Implementation => _definition?.Body != null
        ? new CecilImplementation(this, _definition.Body, _project) as Implementation
        : new EmptyImplementation(this);

    public override Definition Definition => _definition == null
        ? Definition.Unknown
        : _definition.IsAbstract
            ? Definition.Abstract
            : _definition.IsFinal
                ? Definition.Final
                : _definition.IsVirtual
                    ? Definition.Virtual
                    : Definition.Concrete;

    public override string Identifier =>
        $"{Parent.Identifier}.{Name}({string.Join(", ", Arguments.Select(argument => argument.Identifier))})";

    public override string Name => _reference.Name;

    public override IEnumerable<Parameter> Parameters =>
        _reference.GenericParameters.Select(parameter => new CecilParameter(parameter, _project));

    public override Type Parent => new CecilType(_reference.DeclaringType, _project);

    public override Type ReturnType => new CecilType(_reference.ReturnType, _project);

    public override Visibility Visibility => _definition == null
        ? Visibility.Unknown
        : _definition.IsPublic
            ? Visibility.Public
            : _definition.IsPrivate
                ? Visibility.Private
                : _definition.IsFamily
                    ? Visibility.Protected
                    : Visibility.Internal;

    private readonly MethodDefinition _definition;
    private readonly Project _project;
    private readonly MethodReference _reference;

    public CecilMethod(MethodReference reference, Project project)
    {
        if (reference == null)
            throw new ArgumentNullException(nameof(reference));

        if (!(reference is MethodDefinition definition))
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

        _definition = definition;
        _project = project;
        _reference = reference;
    }

    public override bool Equals(Method other)
    {
        return !ReferenceEquals(other, null) && Name == other.Name && Parent == other.Parent &&
               ReturnType == other.ReturnType && Arguments.SequenceEqual(other.Arguments) &&
               Parameters.SequenceEqual(other.Parameters);
    }
}