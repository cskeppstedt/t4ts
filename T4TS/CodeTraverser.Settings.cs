using EnvDTE;
using System;

using T4TS.Builders;

namespace T4TS
{
    public partial class CodeTraverser
    {
        public class TraverserSettings
        {
            public CodeClassInterfaceBuilder InterfaceBuilder { get; set; }
            public CodeEnumBuilder EnumBuilder { get; set; }

            public Func<CodeNamespace, bool> NamespaceFilter { get; set; }
            public Func<CodeClass, bool> ClassFilter { get; set; }
            public Func<CodeEnum, bool> EnumFilter { get; set; }

            public bool ResolveReferences { get; set; }
        }
    }
}
