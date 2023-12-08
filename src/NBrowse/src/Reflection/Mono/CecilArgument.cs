using System;
using System.Text;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono
{
    internal class CecilArgument : Argument
    {
        public override object DefaultValue => this.argument.Constant;

        public override bool HasDefaultValue => this.argument.HasConstant;

        public override string Identifier
        {
            get
            {
                var builder = new StringBuilder();

                switch (this.Modifier)
                {
                    case Modifier.In:
                        builder.Append("in ");

                        break;

                    case Modifier.Out:
                        builder.Append("out ");

                        break;
                }

                builder.Append(this.Type.Identifier);

                if (!string.IsNullOrEmpty(this.Name))
                    builder.Append(" ").Append(this.Name);

                if (this.HasDefaultValue)
                    builder.Append(" = ").Append(this.DefaultValue);

                return builder.ToString();
            }
        }

        public override Modifier Modifier =>
            this.argument.IsIn ? Modifier.In : (this.argument.IsOut ? Modifier.Out : Modifier.None);

        public override string Name => this.argument.Name;

        public override Type Type => new CecilType(this.argument.ParameterType, this.project);

        private readonly ParameterDefinition argument;
        private readonly Project project;

        public CecilArgument(ParameterDefinition argument, Project project)
        {
            this.argument = argument ?? throw new ArgumentNullException(nameof(argument));
            this.project = project;
        }

        public override bool Equals(Argument other)
        {
            return !object.ReferenceEquals(other, null) && this.DefaultValue == other.DefaultValue &&
                   this.HasDefaultValue == other.HasDefaultValue && this.Modifier == other.Modifier &&
                   this.Type == other.Type;
        }
    }
}