using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS
{
    public class MemberOutputAppender : OutputAppender<TypeScriptInterfaceMember>
    {
        public MemberOutputAppender(
            StringBuilder output,
            int baseIndentation,
            Settings settings,
            TypeContext typeContext)
                : base(
                      output,
                      baseIndentation,
                      settings,
                      typeContext)
        {
        }

        public override void AppendOutput(TypeScriptInterfaceMember member)
        {
            AppendIndendation();

            bool isOptional = member.Optional;
            TypeName outputName = this.TypeContext.ResolveOutputTypeName(member.Type);

            //if (member.Type.FullName == typeof(bool).FullName)
            //{
            //    if (Settings.CompatibilityVersion != null && Settings.CompatibilityVersion < new Version(0, 9, 0))
            //        typeName = "bool";
            //    else
            //        typeName = "boolean";
            //}

            Output.AppendFormat("{0}{1}: {2}",
                member.Name,
                (isOptional ? "?" : ""),
                outputName.QualifiedName
            );
            
            Output.AppendLine(";");
        }
    }
}
