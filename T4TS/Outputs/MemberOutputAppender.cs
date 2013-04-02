using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class MemberOutputAppender : OutputAppender<TypeScriptInterfaceMember>
    {
        public MemberOutputAppender(StringBuilder output, int baseIndentation)
            : base(output, baseIndentation)
        {
        }

        public override void AppendOutput(TypeScriptInterfaceMember member)
        {
            AppendIndendation();

            Output.AppendFormat("{0}{1}: {2}",
                member.Name,
                (member.Optional ? "?" : ""),
                member.Type
            );

            Output.AppendLine(";");
        }
    }
}
