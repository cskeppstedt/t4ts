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
            OutputSettings settings,
            TypeContext typeContext,
            bool inGlobalModule)
                : base(
                    settings,
                    typeContext)
        {
            this.InGlobalModule = inGlobalModule;
        }

        public override void AppendOutput(
            StringBuilder output,
            int baseIndentation,
            TypeScriptInterface tsInterface)
        {
            this.BeginInterface(
                output,
                baseIndentation,
                tsInterface);

            this.AppendMembers(
                output,
                baseIndentation,
                tsInterface);

            if (tsInterface.IndexedType != null)
            {
                this.AppendIndexer(
                    output,
                    baseIndentation,
                    tsInterface.IndexedType);
            }

            this.EndInterface(
                output,
                baseIndentation);
        }

        private void AppendMembers(
            StringBuilder output,
            int baseIndentation,
            TypeScriptInterface tsInterface)
        {
            var appender = new MemberOutputAppender(
                this.Settings,
                this.TypeContext);

            if (tsInterface.Fields != null)
            {
                foreach (var field in tsInterface.Fields)
                {
                    appender.AppendOutput(
                        output,
                        baseIndentation + 4,
                        field);
                }
            }

            if (tsInterface.Methods != null
                && tsInterface.Methods.Any())
            {
                MethodAppender emptyAppender = new MethodAppender(
                    this.Settings,
                    this.TypeContext,
                    tsInterface.IsClass);
                foreach (TypeScriptMethod method in tsInterface.Methods)
                {
                    if (method.Appender != null)
                    {
                        method.Appender.AppendOutput(
                            output,
                            baseIndentation + 4,
                            method);
                    }
                    else
                    {
                        emptyAppender.AppendOutput(
                            output,
                            baseIndentation + 4,
                            method);
                    }
                }
            }
        }

        private void BeginInterface(
            StringBuilder output,
            int baseIndentation,
            TypeScriptInterface tsInterface)
        {
            this.AppendIndentedLine(
                output,
                baseIndentation,
                "/** Generated from " + tsInterface.SourceType.RawName + " */");

            TypeName outputName = this.TypeContext.ResolveOutputTypeName(tsInterface);

            this.AppendIndendation(
                output,
                baseIndentation);

            if (!InGlobalModule)
            {
                output.Append("export ");
            }

            if (tsInterface.IsClass)
            {
                output.Append("class ");
            }
            else
            {
                output.Append("interface ");
            }
            output.Append(outputName.QualifiedSimpleName);
            
            if (tsInterface.Parent != null)
            {
                TypeName parentName = this.TypeContext.ResolveOutputTypeName(tsInterface.Parent);
                output.Append(" extends " + parentName.QualifiedName);
            }

            if (!this.Settings.OpenBraceOnNextLine)
            {
                output.AppendLine(" {");
            }
            else
            {
                output.AppendLine();

                this.AppendIndentedLine(
                    output,
                    baseIndentation,
                    "{");
            }
        }

        private void EndInterface(
            StringBuilder output,
            int baseIndentation)
        {
            this.AppendIndentedLine(
                output,
                baseIndentation,
                "}");
        }

        private void AppendIndexer(
            StringBuilder output,
            int baseIndentation,
            TypeReference indexedType)
        {
            TypeName indexedTypeName = this.TypeContext.ResolveOutputTypeName(indexedType);
            this.AppendIndendation(
                output,
                baseIndentation);
            output.AppendFormat(
                "    [index: number]: {0};",
                indexedTypeName.QualifiedName);
            output.AppendLine();
        }
    }
}
