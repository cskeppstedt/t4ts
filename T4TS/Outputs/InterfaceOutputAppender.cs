using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS
{
    public class InterfaceOutputAppender : OutputAppender<TypeScriptInterface>
    {
        private bool InGlobalModule { get; set; }

        public InterfaceOutputAppender(
            StringBuilder output,
            int baseIndentation,
            Settings settings,
            TypeContext typeContext,
            bool inGlobalModule)
                : base(
                    output,
                    baseIndentation,
                    settings,
                    typeContext)
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
            var appender = new MemberOutputAppender(
                this.Output,
                this.BaseIndentation + 4,
                this.Settings,
                this.TypeContext);
            foreach (var member in tsInterface.Members)
                appender.AppendOutput(member);
        }

        private void BeginInterface(TypeScriptInterface tsInterface)
        {
            AppendIndentedLine("/** Generated from " + tsInterface.SourceType.RawName + " **/");

            TypeName outputName = this.TypeContext.ResolveOutputTypeName(tsInterface);

            if (InGlobalModule)
                AppendIndented("interface " + outputName.QualifiedSimpleName);
            else
                AppendIndented("export interface " + outputName.QualifiedSimpleName);
            
            if (tsInterface.Parent != null)
            {
                TypeName parentName = this.TypeContext.ResolveOutputTypeName(tsInterface.Parent);
                Output.Append(" extends " + parentName.QualifiedName);
            }

            Output.AppendLine(" {");
        }

        private void EndInterface()
        {
            AppendIndentedLine("}");
        }

        private void AppendIndexer(TypeReference indexedType)
        {
            TypeName indexedTypeName = this.TypeContext.ResolveOutputTypeName(indexedType);
            AppendIndendation();
            Output.AppendFormat(
                "    [index: number]: {0};",
                indexedTypeName.QualifiedName);
            Output.AppendLine();
        }
    }
}
