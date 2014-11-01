using System;
using System.Text;

namespace T4TS
{
    public class MemberOutputAppender : OutputAppender<TypeScriptInterfaceMember>
    {
        public MemberOutputAppender(StringBuilder output, int baseIndentation, Settings settings)
            : base(output, baseIndentation, settings)
        {
        }

        public override void AppendOutput(TypeScriptInterfaceMember member)
        {
            AppendIndendation();

            var isOptional = member.Optional || (member.Type is NullableType);
            var type = member.Type.ToString();

            if (member.Type is BoolType)
            {
                if (Settings.CompatibilityVersion != null && Settings.CompatibilityVersion < new Version(0, 9, 0))
                    type = "bool";
                else
                    type = "boolean";
            }

            Output.AppendFormat("{0}{1}: {2}",
                member.Name,
                (isOptional ? "?" : ""),
                type
            );
            
            Output.AppendLine(";");
        }
    }
}
