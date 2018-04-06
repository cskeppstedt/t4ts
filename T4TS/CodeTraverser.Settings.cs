using EnvDTE;
using System;

using T4TS.Builders;

namespace T4TS
{
    public partial class CodeTraverser
    {
        public class TraverserSettings
        {
            public ICodeClassToInterfaceBuilder ClassToInterfaceBuilder { get; set; }
            public ICodeInterfaceToInterfaceBuilder InterfaceToInterfaceBuilder { get; set; }
            public CodeEnumBuilder EnumBuilder { get; set; }

            public Func<CodeNamespace, bool> NamespaceFilter { get; set; }
            public Func<CodeClass, bool> ClassFilter { get; set; }
            public Func<CodeInterface, bool> InterfaceFilter { get; set; }
            public Func<CodeEnum, bool> EnumFilter { get; set; }

            public bool ResolveReferences { get; set; }

            public bool FailOnUnresolvedReferences { get; set; }
        }
    }
}
