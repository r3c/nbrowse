using System;
using System.Text;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilArgument : Argument
{
    public override object DefaultValue => _argument.Constant;

    public override bool HasDefaultValue => _argument.HasConstant;

    public override string Identifier
    {
        get
        {
            var builder = new StringBuilder();

            switch (Modifier)
            {
                case Modifier.In:
                    builder.Append("in ");

                    break;

                case Modifier.Out:
                    builder.Append("out ");

                    break;
            }

            builder.Append(Type.Identifier);

            if (!string.IsNullOrEmpty(Name))
                builder.Append(" ").Append(Name);

            if (HasDefaultValue)
                builder.Append(" = ").Append(DefaultValue);

            return builder.ToString();
        }
    }

    public override Modifier Modifier =>
        _argument.IsIn ? Modifier.In : _argument.IsOut ? Modifier.Out : Modifier.None;

    public override string Name => _argument.Name;

    public override Type Type => new CecilType(_argument.ParameterType, _project);

    private readonly ParameterDefinition _argument;
    private readonly Project _project;

    public CecilArgument(ParameterDefinition argument, Project project)
    {
        _argument = argument ?? throw new ArgumentNullException(nameof(argument));
        _project = project;
    }

    public override bool Equals(Argument other)
    {
        return !ReferenceEquals(other, null) && DefaultValue == other.DefaultValue &&
               HasDefaultValue == other.HasDefaultValue && Modifier == other.Modifier &&
               Type == other.Type;
    }
}