using System.Collections.Generic;
using System.Linq;
using NBrowse.Reflection;

namespace NBrowse.Selection;

public static class Usage
{
    public static int CacheSize { get; set; } = 32768;

    private static readonly Cache<(Method, Method), bool> MethodToMethod = new();
    private static readonly Cache<(Method, Type), bool> MethodToType = new();
    private static readonly Cache<(Type, Method), bool> TypeToMethod = new();
    private static readonly Cache<(Type, Type), bool> TypeToType = new();

    public static bool IsUsing(this Method source, Method target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    public static bool IsUsing(this Method source, Type target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    public static bool IsUsing(this Type source, Method target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    public static bool IsUsing(this Type source, Type target, bool includeIndirectUsage = false)
    {
        return IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
    }

    private static bool IsReferencing(Argument source, Type target, State state)
    {
        return IsUsing(source.Type, target, state);
    }

    private static bool IsReferencing(Reflection.Attribute source, Method target, State state)
    {
        return
            IsUsing(source.Constructor, target, state) ||
            IsUsing(source.Type, target, state);
    }

    private static bool IsReferencing(Reflection.Attribute source, Type target, State state)
    {
        return
            IsUsing(source.Constructor, target, state) ||
            IsUsing(source.Type, target, state);
    }

    private static bool IsReferencing(Field source, Type target, State state)
    {
        return IsUsing(source.Type, target, state);
    }

    private static bool IsReferencing(Implementation source, Method target, State state)
    {
        return source.ReferencedMethods.Any(method => IsUsing(method, target, state));
    }

    private static bool IsReferencing(Implementation source, Type target, State state)
    {
        return source.ReferencedTypes.Any(type => IsUsing(type, target, state));
    }

    private static bool IsReferencing(Parameter source, Type target, State state)
    {
        return source.Constraints.Any(constraint => IsUsing(constraint, target, state));
    }

    private static bool IsUsing(Method source, Method target, State state)
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
                 IsReferencing(source.Implementation, target, state)
             ));

        MethodToMethod.Set((source, target), usage);

        return usage;
    }

    private static bool IsUsing(Method source, Type target, State state)
    {
        if (!state.ContinueWith(source))
            return false;

        if (MethodToType.TryGet((source, target), out var usage))
            return usage;

        usage =
            state.TryRecurse() &&
            (
                IsUsing(source.ReturnType, target, state) ||
                source.Arguments.Any(argument => IsReferencing(argument, target, state)) ||
                source.Attributes.Any(attribute => IsReferencing(attribute, target, state)) ||
                source.Parameters.Any(parameter => IsReferencing(parameter, target, state)) ||
                IsReferencing(source.Implementation, target, state)
            );

        MethodToType.Set((source, target), usage);

        return usage;
    }

    private static bool IsUsing(Type source, Method target, State state)
    {
        if (!state.ContinueWith(source))
            return false;

        if (TypeToMethod.TryGet((source, target), out var usage))
            return usage;

        usage = state.TryRecurse() && source.Methods.Any(other => IsUsing(other, target, state));

        TypeToMethod.Set((source, target), usage);

        return usage;
    }

    private static bool IsUsing(Type source, Type target, State state)
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
        private readonly ISet<Method> _methods = new HashSet<Method>();
        private readonly ISet<Type> _types = new HashSet<Type>();

        public State(int depth)
        {
            _depth = depth;
        }

        public bool ContinueWith(Method method)
        {
            return _methods.Add(method);
        }

        public bool ContinueWith(Type type)
        {
            return _types.Add(type);
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