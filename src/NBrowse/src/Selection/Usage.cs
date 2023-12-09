using System.Collections.Generic;
using System.Linq;
using NBrowse.Reflection;

namespace NBrowse.Selection;

public static class Usage
{
    public static int CacheSize { get; set; } = 32768;

    private static readonly Cache<(NMethod, NMethod), bool> MethodToMethod = new();
    private static readonly Cache<(NMethod, NType), bool> MethodToType = new();
    private static readonly Cache<(NType, NMethod), bool> TypeToMethod = new();
    private static readonly Cache<(NType, NType), bool> TypeToType = new();

    public static bool IsUsing(this NMethod source, NMethod target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    public static bool IsUsing(this NMethod source, NType target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    public static bool IsUsing(this NType source, NMethod target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    public static bool IsUsing(this NType source, NType target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    private static bool IsReferencing(NArgument source, NType target, State state)
    {
        return IsUsing(source.NType, target, state);
    }

    private static bool IsReferencing(Reflection.NAttribute source, NMethod target, State state)
    {
        return
            IsUsing(source.Constructor, target, state) ||
            IsUsing(source.NType, target, state);
    }

    private static bool IsReferencing(Reflection.NAttribute source, NType target, State state)
    {
        return
            IsUsing(source.Constructor, target, state) ||
            IsUsing(source.NType, target, state);
    }

    private static bool IsReferencing(NField source, NType target, State state)
    {
        return IsUsing(source.NType, target, state);
    }

    private static bool IsReferencing(NImplementation source, NMethod target, State state)
    {
        return source.ReferencedMethods.Any(method => IsUsing(method, target, state));
    }

    private static bool IsReferencing(NImplementation source, NType target, State state)
    {
        return source.ReferencedTypes.Any(type => IsUsing(type, target, state));
    }

    private static bool IsReferencing(NParameter source, NType target, State state)
    {
        return source.Constraints.Any(constraint => IsUsing(constraint, target, state));
    }

    private static bool IsUsing(NMethod source, NMethod target, State state)
    {
        if (!state.ContinueWith(source))
            return false;

        if (MethodToMethod.TryGet((source, target), out var usage))
            return usage;

        usage =
            source.Equals(target) ||
            (state.TryRecurse() &&
             (
                 source.Attributes.Any(attribute => IsReferencing(attribute, target, state)) ||
                 IsReferencing(source.NImplementation, target, state)
             ));

        MethodToMethod.Set((source, target), usage);

        return usage;
    }

    private static bool IsUsing(NMethod source, NType target, State state)
    {
        if (!state.ContinueWith(source))
            return false;

        if (MethodToType.TryGet((source, target), out var usage))
            return usage;

        usage =
            state.TryRecurse() &&
            (
                IsUsing(source.ReturnNType, target, state) ||
                source.Arguments.Any(argument => IsReferencing(argument, target, state)) ||
                source.Attributes.Any(attribute => IsReferencing(attribute, target, state)) ||
                source.Parameters.Any(parameter => IsReferencing(parameter, target, state)) ||
                IsReferencing(source.NImplementation, target, state)
            );

        MethodToType.Set((source, target), usage);

        return usage;
    }

    private static bool IsUsing(NType source, NMethod target, State state)
    {
        if (!state.ContinueWith(source))
            return false;

        if (TypeToMethod.TryGet((source, target), out var usage))
            return usage;

        usage = state.TryRecurse() && source.Methods.Any(other => IsUsing(other, target, state));

        TypeToMethod.Set((source, target), usage);

        return usage;
    }

    private static bool IsUsing(NType source, NType target, State state)
    {
        if (!state.ContinueWith(source))
            return false;

        if (TypeToType.TryGet((source, target), out var usage))
            return usage;

        var baseOrNull = source.BaseOrNull;

        usage =
            source.Equals(target) ||
            (state.TryRecurse() &&
             (
                 (baseOrNull != null && IsUsing(baseOrNull, target, state)) ||
                 source.Attributes.Any(attribute => IsReferencing(attribute, target, state)) ||
                 source.Fields.Any(field => IsReferencing(field, target, state)) ||
                 source.Interfaces.Any(iface => IsUsing(iface, target, state)) ||
                 source.NestedTypes.Any(type => IsUsing(type, target, state)) ||
                 source.Parameters.Any(parameter => IsReferencing(parameter, target, state)) ||
                 source.Methods.Any(method => IsUsing(method, target, state))
             ));

        TypeToType.Set((source, target), usage);

        return usage;
    }

    /// <summary>
    /// Simple LRU cache used to optimize repeated calls.
    /// </summary>
    private class Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, LinkedListNode<(TKey, TValue)>> _indexed = new();

        private readonly LinkedList<(TKey, TValue)> _ordered = new();

        public void Set(TKey key, TValue value)
        {
            if (_indexed.TryGetValue(key, out var node))
            {
                _ordered.Remove(node);
                _ordered.AddFirst(node);
            }
            else
            {
                node = new((key, value));

                _indexed.Add(key, node);
                _ordered.AddFirst(node);

                while (_ordered.Count > CacheSize)
                {
                    var last = _ordered.Last;

                    _ordered.RemoveLast();
                    _indexed.Remove(last.Value.Item1);
                }
            }
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (!_indexed.TryGetValue(key, out var node))
            {
                value = default;

                return false;
            }

            _ordered.Remove(node);
            _ordered.AddFirst(node);

            value = node.Value.Item2;

            return true;
        }
    }

    /// <summary>
    /// Recursion state used to break out from infinite resolution loops (cyclic references).
    /// </summary>
    private class State
    {
        private int _depth;
        private readonly ISet<NMethod> _methods = new HashSet<NMethod>();
        private readonly ISet<NType> _types = new HashSet<NType>();

        public State(int depth)
        {
            _depth = depth;
        }

        public bool ContinueWith(NMethod nMethod)
        {
            return _methods.Add(nMethod);
        }

        public bool ContinueWith(NType nType)
        {
            return _types.Add(nType);
        }

        public bool TryRecurse()
        {
            if (_depth < 1)
                return false;

            --_depth;

            return true;
        }
    }
}