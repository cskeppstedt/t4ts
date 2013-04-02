using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class ModuleOutputAppender : OutputAppender<TypeScriptModule>
    {
        public ModuleOutputAppender(StringBuilder output, int baseIndentation)
            : base(output, baseIndentation)
        {
        }

        public override void AppendOutput(TypeScriptModule module)
        {
            BeginModule(module);

            var interfaceAppender = new InterfaceOutputAppender(Output, BaseIndentation + 4, module.IsGlobal);
            foreach (var tsInterface in module.Interfaces)
                interfaceAppender.AppendOutput(tsInterface);

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
                Output.Append("module ");
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
