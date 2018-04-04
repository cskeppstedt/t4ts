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
            StringBuilder output,
            int baseIndentation,
            Settings settings,
            TypeContext typeContext)
                : base(
                    output,
                    baseIndentation,
                    settings,
                    typeContext)
        {
        }

        public override void AppendOutput(TypeScriptModule module)
        {
            BeginModule(module);

            InterfaceOutputAppender interfaceAppender = new InterfaceOutputAppender(
                this.Output,
                this.BaseIndentation + 4,
                this.Settings,
                this.TypeContext,
                module.IsGlobal);
            foreach (var tsInterface in module.Interfaces
                .OrderBy((currentInterface) => currentInterface.SourceType.RawName))
            {
                interfaceAppender.AppendOutput(tsInterface);
            }

            EnumOutputAppender enumAppender = new EnumOutputAppender(
                this.Output,
                this.BaseIndentation + 4,
                this.Settings,
                this.TypeContext,
                module.IsGlobal);
            foreach (var tsEnum in module.Enums)
            {
                enumAppender.AppendOutput(tsEnum);
            }

            EndModule(module);
        }

        private void BeginModule(TypeScriptModule module)
        {
            if (module.IsGlobal)
            {
                Output.AppendLine("// -- Begin global interfaces");
            }
            else
            {
                if (Settings.CompatibilityVersion != null && Settings.CompatibilityVersion < new Version(0, 9, 0))
                    Output.Append("module ");
                else
                    Output.Append("declare module ");

                Output.Append(module.QualifiedName);
                Output.AppendLine(" {");
            }
        }

        private void EndModule(TypeScriptModule module)
        {
            if (module.IsGlobal)
                Output.AppendLine("// -- End global interfaces");
            else
                Output.AppendLine("}");
        }
    }
}
