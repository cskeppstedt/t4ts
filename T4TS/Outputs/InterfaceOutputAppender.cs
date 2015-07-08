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
                AppendIndexer(tsInterface);

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
            AppendIndentedLine("/** Generated from " + tsInterface.FullName + " **/");
			
            //replace NewLine characters, so multiline comments will align nicely, and don't stick to the begining of the lines...
            if(!string.IsNullOrWhiteSpace(tsInterface.Comment)) { AppendIndentedLine("/**  " + tsInterface.Comment.Replace(Environment.NewLine, Environment.NewLine + new string(' ', BaseIndentation) + "* ") + " */"); }
            if(!string.IsNullOrWhiteSpace(tsInterface.DocComment)) { AppendIndentedLine("/**  " + tsInterface.DocComment.Replace(Environment.NewLine, Environment.NewLine + new string(' ', BaseIndentation) + "* ") + " */"); }

            if (InGlobalModule)
                AppendIndented("interface " + tsInterface.Name);
            else
                AppendIndented("export interface " + tsInterface.Name);

            if (tsInterface.Parent != null)
                Output.Append(" extends " + (tsInterface.Parent.Module.IsGlobal ? "" : tsInterface.Parent.Module.QualifiedName + ".") + tsInterface.Parent.Name);

            Output.AppendLine(" {");
        }

        private void EndInterface()
        {
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
