using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS
{
    public class MemberOutputAppender : OutputAppender<TypeScriptMember>
    {
        public MemberOutputAppender(
            OutputSettings settings,
            TypeContext typeContext)
                : base(
                      settings,
                      typeContext)
        {
        }

        public override void AppendOutput(
            StringBuilder output,
            int baseIndentation,
            TypeScriptMember member)
        {
            this.AppendIndendation(
                output,
                baseIndentation);

            bool isOptional = member.Optional;
            TypeName outputName = this.TypeContext.ResolveOutputTypeName(member.Type);
            
            output.AppendFormat("{0}{1}: {2}",
                member.Name,
                (isOptional ? "?" : ""),
                outputName.QualifiedName
            );
            
            output.AppendLine(";");
        }
    }
}
