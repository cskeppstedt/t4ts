using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			//replace NewLine characters, so multiline comments will align nicely, and don't stick to the begining of the lines...
            if(!string.IsNullOrWhiteSpace(member.Comment)) { AppendIndentedLine("/**  " + member.Comment.Replace(Environment.NewLine, Environment.NewLine + new string(' ', BaseIndentation) + "* ") + " */"); }
            if(!string.IsNullOrWhiteSpace(member.DocComment)) { AppendIndentedLine("/**  " + member.DocComment.Replace(Environment.NewLine, Environment.NewLine + new string(' ', BaseIndentation) + "* ") + " */"); }

            AppendIndendation();

            bool isOptional = member.Optional;
            string type = member.Type.ToString();

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
