using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NBrowse.Reflection.Empty;

namespace NBrowse.Reflection.Mono;

internal class CecilType : Type
{
    public override IEnumerable<Attribute> Attributes =>
        _definition?.CustomAttributes.Select(attribute => new CecilAttribute(attribute, _project)) ??
        Array.Empty<CecilAttribute>();

    public override IEnumerable<Type> Arguments
    {
        get
        {
            if (!_reference.IsGenericInstance)
                return Array.Empty<Type>();
            return ((GenericInstanceType)_reference).GenericArguments.Select(arg =>
                new CecilType(arg, _project));
        }
    }

    public override Type BaseOrNull => _definition?.BaseType != null
        ? new CecilType(_definition.BaseType, _project)
        : default(Type);

    public override Type ElementOrNull => _reference is ArrayType arrayType
        ? arrayType.ElementType != null ? new CecilType(arrayType.ElementType, _project) : default(Type)
        : _reference is PointerType pointerType
            ? pointerType.ElementType != null
                ? new CecilType(pointerType.ElementType, _project)
                : default(Type)
            : _reference is ByReferenceType byReferenceType
                ? byReferenceType.ElementType != null
                    ? new CecilType(byReferenceType.ElementType, _project)
                    : default(Type)
                : default;

    public override IEnumerable<Field> Fields =>
        _definition?.Fields.Select(field => new CecilField(field, _project)) ?? Array.Empty<CecilField>();

    public override string Identifier =>
        $"{Namespace}{(string.IsNullOrEmpty(Namespace) ? "" : ".")}{Name}";

    public override Definition Definition => _definition == null
        ? Definition.Unknown
        : _definition.IsAbstract
            ? Definition.Abstract
            : _definition.IsSealed
                ? Definition.Final
                : Definition.Virtual;

    public override IEnumerable<Type> Interfaces =>
        _definition?.Interfaces.Select(i => new CecilType(i.InterfaceType, _project)) ??
        Array.Empty<CecilType>();

    public override IEnumerable<Method> Methods =>
        _definition?.Methods.Select(method => new CecilMethod(method, _project)) ??
        Array.Empty<CecilMethod>();

    public override Model Model => _reference.IsValueType
        ? _definition?.IsEnum ?? false ? Model.Enumeration : Model.Structure
        : _definition?.IsInterface ?? false
            ? Model.Interface
            : _reference.IsArray
                ? Model.Array
                : _reference.IsPointer
                    ? Model.Pointer
                    : _reference.IsByReference
                        ? Model.Reference
                        : Model.Class;

    public override string Name => _reference.IsNested
        ? $"{new CecilType(_reference.DeclaringType, _project).Name}+{_reference.Name}"
        : _reference.Name;

    public override string Namespace => _reference.IsNested
        ? new CecilType(_reference.DeclaringType, _project).Namespace
        : _reference.Namespace;

    public override IEnumerable<Type> NestedTypes =>
        _definition?.NestedTypes.Select(type => new CecilType(type, _project)) ?? Array.Empty<CecilType>();

    public override IEnumerable<Parameter> Parameters =>
        _reference.GenericParameters.Select(parameter => new CecilParameter(parameter, _project));

    public override Assembly Parent => _definition != null
        ? new CecilAssembly(_definition.Module.Assembly, _project)
        : EmptyAssembly.Instance;

    public override Visibility Visibility => _definition == null
        ? Visibility.Unknown
        : _definition.IsNested
            ? _definition.IsNestedPublic
                ? Visibility.Public
                : _definition.IsNestedFamily
                    ? Visibility.Protected
                    : _definition.IsNestedPrivate
                        ? Visibility.Private
                        : Visibility.Internal
            : _definition.IsPublic
                ? Visibility.Public
                : _definition.IsNotPublic
                    ? Visibility.Private
                    : Visibility.Internal;

    private readonly TypeDefinition _definition;
    private readonly Project _project;
    private readonly TypeReference _reference;

    public CecilType(TypeReference reference, Project project)
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
        _project = project;
        _reference = reference;
    }

    public override bool Equals(Type other)
    {
        return !ReferenceEquals(other, null) && Namespace == other.Namespace &&
               Name == other.Name && Parameters.SequenceEqual(other.Parameters);
    }
}