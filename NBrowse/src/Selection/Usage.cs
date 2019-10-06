using System.Collections.Generic;
using System.Linq;
using NBrowse.Reflection;

namespace NBrowse.Selection
{
	public static class Usage
	{
		public static int CacheSize { get; set; } = 32768;

		private static readonly Cache<(IMethod, IMethod), bool> MethodToMethod = new Cache<(IMethod, IMethod), bool>();
		private static readonly Cache<(IMethod, IType), bool> MethodToType = new Cache<(IMethod, IType), bool>();
		private static readonly Cache<(IType, IMethod), bool> TypeToMethod = new Cache<(IType, IMethod), bool>();
		private static readonly Cache<(IType, IType), bool> TypeToType = new Cache<(IType, IType), bool>();

		public static bool IsUsing(this IMethod source, IMethod target)
		{
			return Usage.IsReferencing(source, target, new State());
		}

		public static bool IsUsing(this IMethod source, IType target)
		{
			return Usage.IsReferencing(source, target, new State());
		}

		public static bool IsUsing(this IType source, IMethod target)
		{
			return Usage.IsReferencing(source, target, new State());
		}

		public static bool IsUsing(this IType source, IType target)
		{
			return Usage.IsReferencing(source, target, new State());
		}

		private static bool IsReferencing(IArgument source, IType target, State state)
		{
			return Usage.IsReferencing(source.Type, target, state);
		}

		private static bool IsReferencing(IAttribute source, IMethod target, State state)
		{
			return Usage.IsReferencing(source.Constructor, target, state);
		}

		private static bool IsReferencing(IAttribute source, IType target, State state)
		{
			return Usage.IsReferencing(source.Type, target, state);
		}

		private static bool IsReferencing(IImplementation source, IMethod target, State state)
		{
			return source.ReferencedMethods.Any(method => Usage.IsReferencing(method, target, state));
		}

		private static bool IsReferencing(IImplementation source, IType target, State state)
		{
			return source.ReferencedTypes.Any(type => Usage.IsReferencing(type, target, state));
		}

		private static bool IsReferencing(IMethod source, IMethod target, State state)
		{
			if (!state.ContinueWith(source))
				return false;

			if (Usage.MethodToMethod.TryGet((source, target), out var usage))
				return usage;

			var implementationOrNull = source.ImplementationOrNull;

			usage =
				source.Equals(target) ||
				source.Attributes.Any(attribute => Usage.IsReferencing(attribute, target, state)) ||
				implementationOrNull != null && Usage.IsReferencing(implementationOrNull, target, state);

			Usage.MethodToMethod.Set((source, target), usage);

			return usage;
		}

		private static bool IsReferencing(IMethod source, IType target, State state)
		{
			if (!state.ContinueWith(source))
				return false;

			if (Usage.MethodToType.TryGet((source, target), out var usage))
				return usage;

			var implementationOrNull = source.ImplementationOrNull;

			usage =
				Usage.IsReferencing(source.ReturnType, target, state) ||
				source.Arguments.Any(argument => Usage.IsReferencing(argument, target, state)) ||
				source.Attributes.Any(attribute => Usage.IsReferencing(attribute, target, state)) ||
				source.Parameters.Any(parameter => Usage.IsReferencing(parameter, target, state)) ||
				implementationOrNull != null && Usage.IsReferencing(implementationOrNull, target, state);

			Usage.MethodToType.Set((source, target), usage);

			return usage;
		}

		private static bool IsReferencing(IParameter source, IType target, State state)
		{
			return source.Constraints.Any(constraint => Usage.IsReferencing(constraint, target, state));
		}

		private static bool IsReferencing(IType source, IMethod target, State state)
		{
			if (!state.ContinueWith(source))
				return false;

			if (Usage.TypeToMethod.TryGet((source, target), out var usage))
				return usage;

			usage = source.Methods.Any(other => Usage.IsReferencing(other, target, state));

			Usage.TypeToMethod.Set((source, target), usage);

			return usage;
		}

		private static bool IsReferencing(IType source, IType target, State state)
		{
			if (!state.ContinueWith(source))
				return false;

			if (Usage.TypeToType.TryGet((source, target), out var usage))
				return usage;

			var baseOrNull = source.BaseOrNull;

			usage =
				source.Equals(target) ||
				baseOrNull != null && Usage.IsReferencing(baseOrNull, target, state) ||
				source.Attributes.Any(attribute => Usage.IsReferencing(attribute, target, state)) ||
				source.Fields.Any(field => Usage.IsReferencing(field.Type, target, state)) ||
				source.Interfaces.Any(iface => Usage.IsReferencing(iface, target, state)) ||
				source.NestedTypes.Any(type => Usage.IsReferencing(type, target, state)) ||
				source.Parameters.Any(parameter => Usage.IsReferencing(parameter, target, state)) ||
				source.Methods.Any(method => Usage.IsReferencing(method, target, state));

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
			private readonly ISet<IMethod> methods = new HashSet<IMethod>();
			private readonly ISet<IType> types = new HashSet<IType>();

			public bool ContinueWith(IMethod method)
			{
				return this.methods.Add(method);
			}

			public bool ContinueWith(IType type)
			{
				return this.types.Add(type);
			}
		}
	}
}