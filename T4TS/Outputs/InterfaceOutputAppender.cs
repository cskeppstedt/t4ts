using System.Text;

namespace T4TS
{
    public class InterfaceOutputAppender : OutputAppender<TypeScriptInterface>
    {
        public InterfaceOutputAppender(StringBuilder output, int baseIndentation, Settings settings, bool inGlobalModule)
            : base(output, baseIndentation, settings)
        {
            InGlobalModule = inGlobalModule;
        }

        private bool InGlobalModule { get; set; }

        public override void AppendOutput(TypeScriptInterface tsInterface)
        {
            foreach (TypeScriptEnum tsSubEnum in tsInterface.SubEnums)
            {
                AppendOutputSubEnum(tsSubEnum, tsInterface);
            }

            foreach (TypeScriptInterface tsSubClass in tsInterface.SubClasses)
            {
                AppendOutputSubClass(tsSubClass, tsInterface);
            }

            BeginInterface(tsInterface);

            AppendMembers(tsInterface);

            if (tsInterface.IndexedType != null)
                AppendIndexer(tsInterface);

            EndInterface();
        }

        private void AppendOutputSubClass(TypeScriptInterface tsInterface, TypeScriptInterface owner)
        {
            BeginInterface(tsInterface, owner);

            foreach (TypeScriptInterface tsSubClass in tsInterface.SubClasses)
            {
                AppendOutput(tsSubClass);
            }

            AppendMembers(tsInterface, owner);

            if (tsInterface.IndexedType != null)
                AppendIndexer(tsInterface);

            EndInterface(owner);
        }

        private void AppendOutputSubEnum(TypeScriptEnum tsEnum, TypeScriptInterface owner)
        {
            var enumAppender = new EnumOutputAppender(Output, BaseIndentation, Settings);
            enumAppender.AppendOutputSubEnum(tsEnum, owner);
        }

        private void AppendMembers(TypeScriptInterface tsInterface, TypeScriptInterface owner = null)
        {
            int identation = 4;
            while (owner != null)
            {
                identation += 4;
                owner = owner.Owner;
            }
            var appender = new MemberOutputAppender(Output, BaseIndentation + identation, Settings);
            foreach (TypeScriptInterfaceMember member in tsInterface.Members)
                appender.AppendOutput(member);
        }

        private void BeginInterface(TypeScriptInterface tsInterface, TypeScriptInterface owner = null)
        {
            AppendIndentedLine("/** Generated from " + tsInterface.FullName + " **/");

            if (owner == null)
            {
                if (InGlobalModule)
                    AppendIndented("interface " + tsInterface.Name);
                else
                    AppendIndented("export interface " + tsInterface.Name);
            }
            else
            {
                string module = owner.Name;
                string interfaceName = tsInterface.Name;
                string[] arr = tsInterface.Name.Split('.');
                if (arr.Length > 1)
                {
                    module = arr[0];
                    interfaceName = arr[1];
                }
                AppendIndentedLine("export module " + module + " {");
                AppendIndendation();
                AppendIndented("export interface " + interfaceName);
            }

            if (tsInterface.Parent != null)
                Output.Append(" extends " +
                              (tsInterface.Parent.Module.IsGlobal ? "" : tsInterface.Parent.Module.QualifiedName + ".") +
                              tsInterface.Parent.Name);

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

        private void AppendIndexer(TypeScriptInterface tsInterface)
        {
            AppendIndendation();
            Output.AppendFormat("    [index: number]: {0};", tsInterface.IndexedType);
            Output.AppendLine();
        }
    }
}