using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Builders
{
    public class DirectBuilderSettings
    {
        public IDictionary<string, string> NamespaceToModuleMap { get; set; }

        public bool CamelCase { get; set; }
        public bool CreateClasses { get; set; }

        public DirectBuilderSettings()
        {
            this.NamespaceToModuleMap = new Dictionary<string, string>();
        }

        public string GetModuleNameFromNamespace(CodeNamespace codeNamespace)
        {
            string result;
            if (!this.NamespaceToModuleMap.TryGetValue(
                codeNamespace.FullName,
                out result))
            {
                result = codeNamespace.FullName;
            }
            return result;
        }
    }
}
