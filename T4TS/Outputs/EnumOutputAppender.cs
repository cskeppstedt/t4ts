using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public class EnumOutputAppender : OutputAppender<TypeScriptEnum>
    {
        private bool inGlobalModule;

        public EnumOutputAppender(
            OutputSettings settings,
            TypeContext typeContext,
            bool inGlobalModule)
                : base(
                      settings,
                      typeContext)
        {
            this.inGlobalModule = inGlobalModule;
        }

        public override void AppendOutput(
            StringBuilder output,
            int indentation,
            TypeScriptEnum segment)
        {
            this.AppendIndentedLine(
                output,
                indentation,
                "/** Generated from " + segment.SourceType.RawName + " */");

            TypeName outputName = this.TypeContext.ResolveOutputTypeName(segment);
            if (this.inGlobalModule)
            {
                this.AppendIndented(
                    output,
                    indentation,
                    "enum " + outputName.QualifiedSimpleName);
            }
            else
            {
                this.AppendIndented(
                    output,
                    indentation, 
                    "export enum " + outputName.QualifiedSimpleName);
            }

            output.AppendLine(" {");

            this.AppendValues(
                output,
                indentation, 
                segment);

            this.AppendIndentedLine(
                output,
                indentation,
                "}");
        }

        private void AppendValues(
            StringBuilder output,
            int indentation,
            TypeScriptEnum segment)
        {
            bool first = true;
            foreach (var value in segment.Values)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    output.AppendLine(",");
                }

                this.AppendIndented(
                    output,
                    indentation + 4,
                    value.Name);

                if (value.Value != null)
                {
                    output.Append(" = " + value.Value);
                }
            }
            output.AppendLine();
        }
    }
}
