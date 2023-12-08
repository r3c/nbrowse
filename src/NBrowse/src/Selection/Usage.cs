using System.Collections.Generic;
using System.Linq;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
    public static class Usage
    {
        public static int CacheSize { get; set; } = 32768;

        private static readonly Cache<(Method, Method), bool> MethodToMethod = new Cache<(Method, Method), bool>();
        private static readonly Cache<(Method, Type), bool> MethodToType = new Cache<(Method, Type), bool>();
        private static readonly Cache<(Type, Method), bool> TypeToMethod = new Cache<(Type, Method), bool>();
        private static readonly Cache<(Type, Type), bool> TypeToType = new Cache<(Type, Type), bool>();

        public static bool IsUsing(this Method source, Method target, bool includeIndirectUsage = false)
        {
            return Usage.IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
        }

        public static bool IsUsing(this Method source, Type target, bool includeIndirectUsage = false)
        {
            return Usage.IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
        }

        public static bool IsUsing(this Type source, Method target, bool includeIndirectUsage = false)
        {
            return Usage.IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
        }

        public static bool IsUsing(this Type source, Type target, bool includeIndirectUsage = false)
        {
            return Usage.IsUsing(source, target, new State(includeIndirectUsage ? int.MaxValue : 1));
        }

        private static bool IsReferencing(Argument source, Type target, State state)
        {
            return Usage.IsUsing(source.Type, target, state);
        }

        private static bool IsReferencing(Reflection.Attribute source, Method target, State state)
        {
            return
                Usage.IsUsing(source.Constructor, target, state) ||
                Usage.IsUsing(source.Type, target, state);
        }

        private static bool IsReferencing(Reflection.Attribute source, Type target, State state)
        {
            return
                Usage.IsUsing(source.Constructor, target, state) ||
                Usage.IsUsing(source.Type, target, state);
        }

        private static bool IsReferencing(Field source, Type target, State state)
        {
            return Usage.IsUsing(source.Type, target, state);
        }

        private static bool IsReferencing(Implementation source, Method target, State state)
        {
            return source.ReferencedMethods.Any(method => Usage.IsUsing(method, target, state));
        }

        private static bool IsReferencing(Implementation source, Type target, State state)
        {
            return source.ReferencedTypes.Any(type => Usage.IsUsing(type, target, state));
        }

        private static bool IsReferencing(Parameter source, Type target, State state)
        {
            return source.Constraints.Any(constraint => Usage.IsUsing(constraint, target, state));
        }

        private static bool IsUsing(Method source, Method target, State state)
        {
            if (!state.ContinueWith(source))
                return false;

            if (Usage.MethodToMethod.TryGet((source, target), out var usage))
                return usage;

            usage =
                source.Equals(target) ||
                state.TryRecurse() &&
                (
                    source.Attributes.Any(attribute => Usage.IsReferencing(attribute, target, state)) ||
                    Usage.IsReferencing(source.Implementation, target, state)
                );

            Usage.MethodToMethod.Set((source, target), usage);

            return usage;
        }

        private static bool IsUsing(Method source, Type target, State state)
        {
            if (!state.ContinueWith(source))
                return false;

            if (Usage.MethodToType.TryGet((source, target), out var usage))
                return usage;

            usage =
                state.TryRecurse() &&
                (
                    Usage.IsUsing(source.ReturnType, target, state) ||
                    source.Arguments.Any(argument => Usage.IsReferencing(argument, target, state)) ||
                    source.Attributes.Any(attribute => Usage.IsReferencing(attribute, target, state)) ||
                    source.Parameters.Any(parameter => Usage.IsReferencing(parameter, target, state)) ||
                    Usage.IsReferencing(source.Implementation, target, state)
                );

            Usage.MethodToType.Set((source, target), usage);

            return usage;
        }

        private static bool IsUsing(Type source, Method target, State state)
        {
            if (!state.ContinueWith(source))
                return false;

            if (Usage.TypeToMethod.TryGet((source, target), out var usage))
                return usage;

            usage = state.TryRecurse() && source.Methods.Any(other => Usage.IsUsing(other, target, state));

            Usage.TypeToMethod.Set((source, target), usage);

            return usage;
        }

        private static bool IsUsing(Type source, Type target, State state)
        {
            if (!state.ContinueWith(source))
                return false;

            if (Usage.TypeToType.TryGet((source, target), out var usage))
                return usage;

            var baseOrNull = source.BaseOrNull;

            usage =
                source.Equals(target) ||
                state.TryRecurse() &&
                (
                    baseOrNull != null && Usage.IsUsing(baseOrNull, target, state) ||
                    source.Attributes.Any(attribute => Usage.IsReferencing(attribute, target, state)) ||
                    source.Fields.Any(field => Usage.IsReferencing(field, target, state)) ||
                    source.Interfaces.Any(iface => Usage.IsUsing(iface, target, state)) ||
                    source.NestedTypes.Any(type => Usage.IsUsing(type, target, state)) ||
                    source.Parameters.Any(parameter => Usage.IsReferencing(parameter, target, state)) ||
                    source.Methods.Any(method => Usage.IsUsing(method, target, state))
                );

            Usage.TypeToType.Set((source, target), usage);

            return usage;
        }

        /// <summary>
        /// Simple LRU cache used to optimize repeated calls.
        /// </summary>
        private class Cache<TKey, TValue>
        {
            private readonly Dictionary<TKey, LinkedListNode<(TKey, TValue)>> indexed =
                new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>();

            private readonly LinkedList<(TKey, TValue)> ordered = new LinkedList<(TKey, TValue)>();

            public void Set(TKey key, TValue value)
            {
                if (this.indexed.TryGetValue(key, out var node))
                {
                    this.ordered.Remove(node);
                    this.ordered.AddFirst(node);
                }
                else
                {
                    node = new LinkedListNode<(TKey, TValue)>((key, value));

                    this.indexed.Add(key, node);
                    this.ordered.AddFirst(node);

                    while (this.ordered.Count > Usage.CacheSize)
                    {
                        var last = this.ordered.Last;

                        this.ordered.RemoveLast();
                        this.indexed.Remove(last.Value.Item1);
                    }
                }
            }

            public bool TryGet(TKey key, out TValue value)
            {
                if (!this.indexed.TryGetValue(key, out var node))
                {
                    value = default;

                    return false;
                }

                this.ordered.Remove(node);
                this.ordered.AddFirst(node);

                value = node.Value.Item2;

                return true;
            }
        }

        /// <summary>
        /// Recursion state used to break out from infinite resolution loops (cyclic references).
        /// </summary>
        private class State
        {
            private int depth;
            private readonly ISet<Method> methods = new HashSet<Method>();
            private readonly ISet<Type> types = new HashSet<Type>();

            public State(int depth)
            {
                this.depth = depth;
            }

            public bool ContinueWith(Method method)
            {
                return this.methods.Add(method);
            }

            public bool ContinueWith(Type type)
            {
                return this.types.Add(type);
            }

            public bool TryRecurse()
            {
                if (this.depth < 1)
                    return false;

                --this.depth;

                return true;
            }
        }
    }
}