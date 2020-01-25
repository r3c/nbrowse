using System;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
	internal class CecilArgument : Argument
	{
		public override object DefaultValue => this.argument.Constant;

		public override bool HasDefaultValue => this.argument.HasConstant;

		public override string Identifier => $"{this.Type.Identifier} {this.Name}";

		public override Modifier Modifier =>
			this.argument.IsIn ? Modifier.In : (this.argument.IsOut ? Modifier.Out : Modifier.None);

		public override string Name => this.argument.Name;

		public override Type Type => new CecilType(this.argument.ParameterType, this.parent);

		private readonly ParameterDefinition argument;
		private readonly Assembly parent;

		public CecilArgument(ParameterDefinition argument, Assembly parent)
		{
			this.argument = argument ?? throw new ArgumentNullException(nameof(argument));
			this.parent = parent;
		}

		public override bool Equals(Argument other)
		{
			// FIXME: inaccurate, waiting for https://github.com/jbevain/cecil/issues/389
			return !object.ReferenceEquals(other, null) && this.Identifier == other.Identifier;
		}
	}
}
