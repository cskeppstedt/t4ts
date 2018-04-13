using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using T4TS.Outputs;

namespace T4TS.Builders
{
    public class DirectEnumBuilder : CodeEnumBuilder
    {
        private DirectBuilderSettings settings;

        public DirectEnumBuilder(DirectBuilderSettings settings)
        {
            this.settings = settings;
        }

        public TypeScriptEnum Build(
            CodeEnum codeEnum,
            TypeContext typeContext)
        {
            string moduleName = this.settings.GetModuleNameFromNamespace(codeEnum.Namespace);

            bool enumCreated;
            TypeScriptEnum result = typeContext.GetOrCreateEnum(
                moduleName,
                TypeName.ParseDte(codeEnum.FullName),
                codeEnum.Name,
                out enumCreated);

            foreach (CodeVariable member in codeEnum.Members)
            {
                result.Values.Add(new TypeScriptEnumValue()
                    {
                        Name = member.Name,
                        Value = (member.InitExpression != null)
                            ? member.InitExpression.ToString()
                            : null
                    });
            }

            return result;
        }
    }
}
