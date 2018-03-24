using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class InterfaceOutputAppender : OutputAppender<TypeScriptInterface>
    {
        private bool InGlobalModule { get; set; }

        public InterfaceOutputAppender(StringBuilder output, int baseIndentation, Settings settings, bool inGlobalModule)
            : base(output, baseIndentation, settings)
        {
            this.InGlobalModule = inGlobalModule;
        }

        public override void AppendOutput(TypeScriptInterface tsInterface)
        {
            BeginInterface(tsInterface);

            AppendMembers(tsInterface);
            
            if (tsInterface.IndexedType != null)
                AppendIndexer(tsInterface.IndexedType);

            EndInterface();
        }

        private void AppendMembers(TypeScriptInterface tsInterface)
        {
            var appender = new MemberOutputAppender(Output, BaseIndentation + 4, Settings);
            foreach (var member in tsInterface.Members)
                appender.AppendOutput(member);
        }

        private void BeginInterface(TypeScriptInterface tsInterface)
        {
            AppendIndentedLine("/** Generated from " + tsInterface.SourceType.RawName + " **/");

            TypeName outputName = tsInterface.SourceType.ReplaceUnqualifiedName(tsInterface.Name);

            if (InGlobalModule)
                AppendIndented("interface " + outputName.QualifiedName);
            else
                AppendIndented("export interface " + outputName.QualifiedName);

            // In some cases the Parent is a complex type, like a List<> that we don't want to define as a base type.
            // Those types end up with a null Name.  There should be a better way to handle that.
            if (tsInterface.Parent != null
                && !String.IsNullOrEmpty(tsInterface.Parent.Name))
            {
                string parentModuleName = (tsInterface.Parent.Module != null)
                    ? tsInterface.Parent.Module.QualifiedName + "."
                    : String.Empty;
                Output.Append(" extends " + parentModuleName + tsInterface.Parent.Name);
            }

            Output.AppendLine(" {");
        }

        private void EndInterface()
        {
            AppendIndentedLine("}");
        }

        private void AppendIndexer(TypeScriptOutputType indexedType)
        {
            AppendIndendation();
            string parentModuleName = (indexedType.Module != null)
                ? indexedType.Module.QualifiedName + "."
                : String.Empty;
            Output.AppendFormat(
                "    [index: number]: {0}{1};",
                parentModuleName,
                indexedType.Name);
            Output.AppendLine();
        }
    }
}
