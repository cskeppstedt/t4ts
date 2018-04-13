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

            if (!this.Settings.OrderInterfacesByReference)
            {
                this.AppendInterfacesInCreatedOrder(
                    output,
                    baseIndentation + 4,
                    module);
            }
            else
            {
                this.AppendInterfacesInReferenceOrder(
                    output,
                    baseIndentation + 4,
                    module);
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
                if (!this.Settings.IsDeclaration
                    || (Settings.CompatibilityVersion != null
                        && Settings.CompatibilityVersion < new Version(0, 9, 0)))
                {
                    output.Append("module ");
                }
                else
                {
                    output.Append("declare module ");
                }

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

        private void AppendInterfacesInCreatedOrder(
            StringBuilder output,
            int indent,
            TypeScriptModule module)
        {
            InterfaceOutputAppender interfaceAppender = new InterfaceOutputAppender(
                this.Settings,
                this.TypeContext,
                module.IsGlobal);

            foreach (var tsInterface in module.Interfaces
                .OrderBy((currentInterface) => currentInterface.SourceType.RawName))
            {
                interfaceAppender.AppendOutput(
                    output,
                    indent,
                    tsInterface);
            }
        }

        private void AppendInterfacesInReferenceOrder(
            StringBuilder output,
            int indent,
            TypeScriptModule module)
        {
            InterfaceOutputAppender interfaceAppender = new InterfaceOutputAppender(
                this.Settings,
                this.TypeContext,
                module.IsGlobal);

            HashSet<string> outputTypeNames = new HashSet<string>();
            int handledCount = 0;
            IList<TypeReference> allTypeReferences = new List<TypeReference>(this.TypeContext.GetTypeReferences());
            while (handledCount < allTypeReferences.Count)
            {
                for (; handledCount < allTypeReferences.Count; handledCount++)
                {
                    this.AppendType(
                        output,
                        indent,
                        allTypeReferences[handledCount],
                        outputTypeNames,
                        interfaceAppender,
                        module);
                }

                allTypeReferences = new List<TypeReference>(this.TypeContext.GetTypeReferences());
            }

            foreach (TypeScriptInterface currentInterface in module.Interfaces)
            {
                if (!outputTypeNames.Contains(currentInterface.SourceType.UniversalName))
                {
                    interfaceAppender.AppendOutput(
                        output,
                        indent,
                        currentInterface);
                }
            }
        }

        private void AppendType(
            StringBuilder output,
            int indent,
            TypeReference typeReference,
            HashSet<string> outputTypeNames,
            InterfaceOutputAppender interfaceAppender,
            TypeScriptModule currentModule)
        {
            if (!outputTypeNames.Contains(typeReference.SourceType.UniversalName))
            {
                TypeScriptInterface interfaceType = this.TypeContext.GetInterface(typeReference.SourceType);
                if (interfaceType != null
                    && currentModule.Interfaces.Contains(interfaceType))
                {
                    if (interfaceType.Parent != null)
                    {
                        this.AppendType(
                            output,
                            indent,
                            interfaceType.Parent,
                            outputTypeNames,
                            interfaceAppender,
                            currentModule);
                    }

                    // More reference handling...

                    if (!outputTypeNames.Contains(typeReference.SourceType.UniversalName))
                    { 
                        interfaceAppender.AppendOutput(
                            output,
                            indent,
                            interfaceType);
                        outputTypeNames.Add(typeReference.SourceType.UniversalName);
                    }
                }
            }
        }
    }
}
