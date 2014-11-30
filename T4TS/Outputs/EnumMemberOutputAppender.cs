using System.Text;

namespace T4TS
{
    public class EnumMemberOutputAppender : OutputAppender<TypeScriptEnumMember>
    {
        public EnumMemberOutputAppender(StringBuilder output, int baseIndentation, Settings settings)
            : base(output, baseIndentation, settings)
        {
        }

        public override void AppendOutput(TypeScriptEnumMember member)
        {
            AppendIndendation();

            Output.AppendFormat("{0} = {1}",
                member.Name,
                member.Value
                );

            Output.AppendLine(",");
        }
    }
}