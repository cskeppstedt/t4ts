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
            this.inGlobalModule = inGlobalModule;
        }

        public override void AppendOutput(TypeScriptEnum segment)
        {
            AppendIndentedLine("/** Generated from " + segment.SourceType.RawName + " **/");

            TypeName outputName = this.TypeContext.ResolveOutputTypeName(segment);
            if (this.inGlobalModule)
                AppendIndented("enum " + outputName.QualifiedSimpleName);
            else
                AppendIndented("export enum " + outputName.QualifiedSimpleName);
            
            Output.AppendLine(" {");

            AppendValues(segment);

            AppendIndentedLine("}");
        }

        private void AppendValues(TypeScriptEnum segment)
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
                    Output.AppendLine(",");
                }

                AppendIndented("    " + value.Name);

                if (value.Value != null)
                {
                    Output.Append(" = " + value.Value);
                }
            }
            Output.AppendLine();
        }
    }
}
