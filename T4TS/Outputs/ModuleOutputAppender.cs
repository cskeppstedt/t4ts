using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Outputs;

namespace T4TS
{
    public class ModuleOutputAppender : OutputAppender<TypeScriptModule>
    {
        public ModuleOutputAppender(
            OutputSettings settings,
            TypeContext typeContext)
                : base(
                    settings,
                    typeContext)
        {
        }

        public override void AppendOutput(
            StringBuilder output,
            int baseIndentation,
            TypeScriptModule module)
        {
            BeginModule(
                output,
                baseIndentation,
                module);

            InterfaceOutputAppender interfaceAppender = new InterfaceOutputAppender(
                this.Settings,
                this.TypeContext,
                module.IsGlobal);
            foreach (var tsInterface in module.Interfaces
                .OrderBy((currentInterface) => currentInterface.SourceType.RawName))
            {
                interfaceAppender.AppendOutput(
                    output,
                    baseIndentation + 4,
                    tsInterface);
            }

            EnumOutputAppender enumAppender = new EnumOutputAppender(
                this.Settings,
                this.TypeContext,
                module.IsGlobal);
            foreach (var tsEnum in module.Enums)
            {
                enumAppender.AppendOutput(
                    output,
                    baseIndentation + 4, 
                    tsEnum);
            }

            this.EndModule(
                output,
                module);
        }

        private void BeginModule(
            StringBuilder output,
            int indent,
            TypeScriptModule module)
        {
            if (module.IsGlobal)
            {
                output.AppendLine("// -- Begin global interfaces");
            }
            else
            {
                if (Settings.CompatibilityVersion != null && Settings.CompatibilityVersion < new Version(0, 9, 0))
                    output.Append("module ");
                else
                    output.Append("declare module ");

                output.Append(module.QualifiedName);

                if (!this.Settings.OpenBraceOnNextLine)
                {
                    output.AppendLine(" {");
                }
                else
                {
                    output.AppendLine();

                    this.AppendIndentedLine(
                        output,
                        indent,
                        "{");
                }
            }
        }

        private void EndModule(
            StringBuilder output,
            TypeScriptModule module)
        {
            if (module.IsGlobal)
                output.AppendLine("// -- End global interfaces");
            else
                output.AppendLine("}");
        }
    }
}
