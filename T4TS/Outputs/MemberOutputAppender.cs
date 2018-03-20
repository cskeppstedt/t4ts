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
            AppendIndendation();

            bool isOptional = member.Optional;
            string typeName = member.Type.Name;

            if (member.Type.FullName == typeof(bool).FullName)
            {
                if (Settings.CompatibilityVersion != null && Settings.CompatibilityVersion < new Version(0, 9, 0))
                    typeName = "bool";
                else
                    typeName = "boolean";
            }

            if (member.Type.Module != null)
            {
                typeName = String.Format(
                    "{0}.{1}",
                    member.Type.Module.QualifiedName,
                    typeName);
            }

            Output.AppendFormat("{0}{1}: {2}",
                member.Name,
                (isOptional ? "?" : ""),
                typeName
            );
            
            Output.AppendLine(";");
        }
    }
}
