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

            if (InGlobalModule)
                AppendIndented("interface " + tsInterface.Name);
            else
                AppendIndented("export interface " + tsInterface.Name);

            if (tsInterface.GenericParameters != null)
            {
                bool firstGenericParameter = true;
                foreach(string genericParameter in tsInterface.GenericParameters)
                {
                    if (firstGenericParameter)
                    {
                        Output.Append("<");
                        firstGenericParameter = false;
                    }
                    else
                    {
                        Output.Append(", ");
                    }
                    Output.Append(genericParameter);
                }
                if (!firstGenericParameter)
                {
                    Output.Append(">");
                }
            }

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

        private void AppendIndexer(TypeScriptInterface tsInterface)
        {
            AppendIndendation();
            Output.AppendFormat("    [index: number]: {0};", tsInterface.IndexedType);
            Output.AppendLine();
        }
    }
}
