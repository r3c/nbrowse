using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection
{
	public abstract class Assembly : IEquatable<Assembly>
	{
		public static bool operator ==(Assembly lhs, Assembly rhs)
		{
			return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
		}

		public static bool operator !=(Assembly lhs, Assembly rhs)
		{
			return !lhs?.Equals(rhs) ?? !ReferenceEquals(rhs, null);
		}

		[Description("Custom attributes (resolved assembly only)")]
		public abstract IEnumerable<Attribute> Attributes { get; }

		[Description("Name of assembly culture")]
		public abstract string Culture { get; }

		[Description("File name on disk (resolved assembly only)")]
		public abstract string FileName { get; }

		[Description("Unique human-readable identifier")]
		public abstract string Identifier { get; }

		[Description("Assembly name")]
		public abstract string Name { get; }

		[Description("Referenced assemblies (resolved assembly only)")]
		[JsonIgnore]
		public abstract IEnumerable<Assembly> References { get; }

		[Description("Assembly version")]
		public abstract Version Version { get; }

		[Description("Declared types (resolved assembly only)")]
		[JsonIgnore]
		public abstract IEnumerable<Type> Types { get; }

		public abstract bool Equals(Assembly other);

		public override bool Equals(object obj)
		{
			return obj is Assembly other && this.Equals(other);
		}

		public override int GetHashCode()
		{
			return this.Identifier.GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Assembly={this.Identifier}}}";
		}
	}
}