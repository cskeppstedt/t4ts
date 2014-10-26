using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class EnumOutputAppender : OutputAppender<TypeScriptEnum>
    {
        public EnumOutputAppender(StringBuilder output, int baseIndentation, Settings settings)
            : base(output, baseIndentation, settings)
        {
        }

        public override void AppendOutput(TypeScriptEnum tsEnum)
        {
            BeginInterface(tsEnum);

            AppendMembers(tsEnum);
            
            EndInterface();
        }

        private void AppendMembers(TypeScriptEnum tsEnum)
        {
            var appender = new EnumMemberOutputAppender(Output, BaseIndentation + 4, Settings);
            foreach (var member in tsEnum.Members)
                appender.AppendOutput(member);
        }

        private void BeginInterface(TypeScriptEnum tsEnum)
        {
            AppendIndentedLine("/** Generated from " + tsEnum.FullName + " **/");

            AppendIndented("enum " + tsEnum.Name);

            Output.AppendLine(" {");
        }

        private void EndInterface()
        {
            AppendIndentedLine("}");
        }
    }
}
