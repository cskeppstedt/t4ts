using System.Text;

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

        public void AppendOutputSubEnum(TypeScriptEnum tsEnum, TypeScriptInterface owner)
        {
            BeginInterface(tsEnum, owner);

            AppendMembers(tsEnum, owner);

            EndInterface(owner);
        }

        private void AppendMembers(TypeScriptEnum tsEnum, TypeScriptInterface owner = null)
        {
            int identation = 4;
            while (owner != null)
            {
                identation += 4;
                owner = owner.Owner;
            }
            var appender = new EnumMemberOutputAppender(Output, BaseIndentation + identation, Settings);
            foreach (TypeScriptEnumMember member in tsEnum.Members)
                appender.AppendOutput(member);
        }

        private void BeginInterface(TypeScriptEnum tsEnum, TypeScriptInterface owner = null)
        {
            AppendIndentedLine("/** Generated from " + tsEnum.FullName + " **/");
            if (owner == null)
            {
                AppendIndented("const enum " + tsEnum.Name);
            }
            else
            {
                string module = owner.Name;
                string enumName = tsEnum.Name;
                string[] arr = tsEnum.Name.Split('.');
                if (arr.Length > 1)
                {
                    module = arr[0];
                    enumName = arr[1];
                }
                AppendIndentedLine("export module " + module + " {");
                AppendIndendation();
                AppendIndented("const enum " + enumName);
            }
            Output.AppendLine(" {");
        }

        private void EndInterface(TypeScriptInterface owner = null)
        {
            while (owner != null)
            {
                AppendIndendation();
                AppendIndentedLine("}");
                owner = owner.Owner;
            }
            AppendIndentedLine("}");
        }
    }
}