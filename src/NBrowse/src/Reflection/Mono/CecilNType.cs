using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Reflection.Empty;

namespace NBrowse.Reflection.Mono;

internal class CecilNType : NType
{
    public override IEnumerable<NAttribute> Attributes =>
        _definition?.CustomAttributes.Select(attribute => new CecilNAttribute(attribute, _nProject)) ??
        Array.Empty<CecilNAttribute>();

    public override IEnumerable<NType> Arguments
    {
        get
        {
            if (!_reference.IsGenericInstance)
                return Array.Empty<NType>();
            return ((GenericInstanceType)_reference).GenericArguments.Select(arg =>
                new CecilNType(arg, _nProject));
        }
    }

    public override NType BaseOrNull => _definition?.BaseType != null
        ? new CecilNType(_definition.BaseType, _nProject)
        : default(NType);

    public override NType ElementOrNull => _reference is ArrayType arrayType
        ? arrayType.ElementType != null ? new CecilNType(arrayType.ElementType, _nProject) : default(NType)
        : _reference is PointerType pointerType
            ? pointerType.ElementType != null
                ? new CecilNType(pointerType.ElementType, _nProject)
                : default(NType)
            : _reference is ByReferenceType byReferenceType
                ? byReferenceType.ElementType != null
                    ? new CecilNType(byReferenceType.ElementType, _nProject)
                    : default(NType)
                : default;

    public override IEnumerable<NField> Fields =>
        _definition?.Fields.Select(field => new CecilNField(field, _nProject)) ?? Array.Empty<CecilNField>();

    public override string Identifier =>
        $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";

    public override NDefinition NDefinition => _definition == null
        ? NDefinition.Unknown
        : _definition.IsAbstract
            ? NDefinition.Abstract
            : _definition.IsSealed
                ? NDefinition.Final
                : NDefinition.Virtual;

    public override IEnumerable<NType> Interfaces =>
        _definition?.Interfaces.Select(i => new CecilNType(i.InterfaceType, _nProject)) ??
        Array.Empty<CecilNType>();

    public override IEnumerable<NMethod> Methods =>
        _definition?.Methods.Select(method => new CecilNMethod(method, _nProject)) ??
        Array.Empty<CecilNMethod>();

    public override NModel NModel => _reference.IsValueType
        ? _definition?.IsEnum ?? false ? NModel.Enumeration : NModel.Structure
        : _definition?.IsInterface ?? false
            ? NModel.Interface
            : _reference.IsArray
                ? NModel.Array
                : _reference.IsPointer
                    ? NModel.Pointer
                    : _reference.IsByReference
                        ? NModel.Reference
                        : NModel.Class;

    public override string Name => _reference.IsNested
        ? $"{new CecilNType(_reference.DeclaringType, _nProject).Name}+{_reference.Name}"
        : _reference.Name;

    public override string Namespace => _reference.IsNested
        ? new CecilNType(_reference.DeclaringType, _nProject).Namespace
        : _reference.Namespace;

    public override IEnumerable<NType> NestedTypes =>
        _definition?.NestedTypes.Select(type => new CecilNType(type, _nProject)) ?? Array.Empty<CecilNType>();

    public override IEnumerable<NParameter> Parameters =>
        _reference.GenericParameters.Select(parameter => new CecilNParameter(parameter, _nProject));

    public override NAssembly Parent => _definition != null
        ? new CecilNAssembly(_definition.Module.Assembly, _nProject)
        : EmptyNAssembly.Instance;

    public override NVisibility NVisibility => _definition == null
        ? NVisibility.Unknown
        : _definition.IsNested
            ? _definition.IsNestedPublic
                ? NVisibility.Public
                : _definition.IsNestedFamily
                    ? NVisibility.Protected
                    : _definition.IsNestedPrivate
                        ? NVisibility.Private
                        : NVisibility.Internal
            : _definition.IsPublic
                ? NVisibility.Public
                : _definition.IsNotPublic
                    ? NVisibility.Private
                    : NVisibility.Internal;

    private readonly TypeDefinition _definition;
    private readonly NProject _nProject;
    private readonly TypeReference _reference;

    public CecilNType(TypeReference reference, NProject nProject)
    {
        if (reference == null)
            throw new ArgumentNullException(nameof(reference));

        if (!(reference is TypeDefinition definition))
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

    public override bool Equals(NType other)
    {
        return !ReferenceEquals(other, null) && Namespace == other.Namespace &&
               Name == other.Name && Parameters.SequenceEqual(other.Parameters);
    }
}