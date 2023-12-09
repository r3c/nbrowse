using System;
using System.Text;
using Mono.Cecil;

namespace NBrowse.Reflection.Mono;

internal class CecilNArgument : NArgument
{
    public override object DefaultValue => _argument.Constant;

    public override bool HasDefaultValue => _argument.HasConstant;

    public override string Identifier
    {
        get
        {
            var builder = new StringBuilder();

            switch (NModifier)
            {
                case NModifier.In:
                    builder.Append("in ");

                    break;

                case NModifier.Out:
                    builder.Append("out ");

                    break;
            }

            builder.Append(NType.Identifier);

            if (!string.IsNullOrEmpty(Name))
                builder.Append(" ").Append(Name);

            if (HasDefaultValue)
                builder.Append(" = ").Append(DefaultValue);

            return builder.ToString();
        }
    }

    public override NModifier NModifier =>
        _argument.IsIn ? NModifier.In : _argument.IsOut ? NModifier.Out : NModifier.None;

    public override string Name => _argument.Name;

    public override NType NType => new CecilNType(_argument.ParameterType, _nProject);

    private readonly ParameterDefinition _argument;
    private readonly NProject _nProject;

    public CecilNArgument(ParameterDefinition argument, NProject nProject)
    {
        _argument = argument ?? throw new ArgumentNullException(nameof(argument));
        _nProject = nProject;
    }

    public override bool Equals(NArgument other)
    {
        return !ReferenceEquals(other, null) && DefaultValue == other.DefaultValue &&
               HasDefaultValue == other.HasDefaultValue && NModifier == other.NModifier &&
               NType == other.NType;
    }
}