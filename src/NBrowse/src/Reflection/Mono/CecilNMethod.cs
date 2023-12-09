using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Reflection.Empty;

namespace NBrowse.Reflection.Mono;

internal class CecilNMethod : NMethod
{
    public override IEnumerable<NArgument> Arguments =>
        _reference.Parameters.Select(argument => new CecilNArgument(argument, _nProject));

    public override IEnumerable<NAttribute> Attributes =>
        _definition?.CustomAttributes.Select(attribute => new CecilNAttribute(attribute, _nProject)) ??
        Array.Empty<CecilNAttribute>();

    public override NBinding NBinding => _definition == null
        ? NBinding.Unknown
        : _definition.IsConstructor
            ? NBinding.Constructor
            : _definition.IsStatic
                ? NBinding.Static
                : NBinding.Instance;

    public override NImplementation NImplementation => _definition?.Body != null
        ? new CecilNImplementation(this, _definition.Body, _nProject)
        : new EmptyNImplementation(this);

    public override NDefinition NDefinition => _definition == null
        ? NDefinition.Unknown
        : _definition.IsAbstract
            ? NDefinition.Abstract
            : _definition.IsFinal
                ? NDefinition.Final
                : _definition.IsVirtual
                    ? NDefinition.Virtual
                    : NDefinition.Concrete;

    public override string Identifier =>
        $"{Parent.Identifier}.{Name}({string.Join(", ", Arguments.Select(argument => argument.Identifier))})";

    public override string Name => _reference.Name;

    public override IEnumerable<NParameter> Parameters =>
        _reference.GenericParameters.Select(parameter => new CecilNParameter(parameter, _nProject));

    public override NType Parent => new CecilNType(_reference.DeclaringType, _nProject);

    public override NType ReturnNType => new CecilNType(_reference.ReturnType, _nProject);

    public override NVisibility NVisibility => _definition == null
        ? NVisibility.Unknown
        : _definition.IsPublic
            ? NVisibility.Public
            : _definition.IsPrivate
                ? NVisibility.Private
                : _definition.IsFamily
                    ? NVisibility.Protected
                    : NVisibility.Internal;

    private readonly MethodDefinition _definition;
    private readonly NProject _nProject;
    private readonly MethodReference _reference;

    public CecilNMethod(MethodReference reference, NProject nProject)
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
        _nProject = nProject;
        _reference = reference;
    }

    public override bool Equals(NMethod other)
    {
        return !ReferenceEquals(other, null) && Name == other.Name && Parent == other.Parent &&
               ReturnNType == other.ReturnNType && Arguments.SequenceEqual(other.Arguments) &&
               Parameters.SequenceEqual(other.Parameters);
    }
}