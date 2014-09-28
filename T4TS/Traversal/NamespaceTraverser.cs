using EnvDTE;
using System;
using System.Linq;

namespace T4TS
{
    public class NamespaceTraverser
    {
        public Action<CodeClass> WithCodeClass { get; private set; }

        public NamespaceTraverser(CodeNamespace ns, Action<CodeClass> withCodeClass)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");
            
            if (withCodeClass == null)
                throw new ArgumentNullException("withCodeClass");
            
            WithCodeClass = withCodeClass;
            
            if (ns.Members != null)
                Traverse(ns.Members);
        }

        private void Traverse(CodeElements members)
        {
            foreach (object elem in members)
            {
                if (elem is CodeClass)
                    WithCodeClass((CodeClass)elem);
            }
        }
    }
}
