using EnvDTE;
using System;
using System.Linq;

namespace T4TS
{
    public class NamespaceTraverser
    {
        public Action<CodeClass> WithCodeClass { get; private set; }
        public Action<CodeEnum> WithCodeEnum { get; private set; }

        public NamespaceTraverser(CodeNamespace ns, Action<CodeClass> withCodeClass, Action<CodeEnum> withCodeEnum)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");
            
            if (withCodeClass == null)
                throw new ArgumentNullException("withCodeClass");
            
            WithCodeClass = withCodeClass;

            WithCodeEnum = withCodeEnum;
            
            if (ns.Members != null)
                Traverse(ns.Members);
        }

        private void Traverse(CodeElements members)
        {
            foreach (var codeEnum in members.OfType<CodeEnum>())
                WithCodeEnum(codeEnum);
            foreach (var codeClass in members.OfType<CodeClass>())
                WithCodeClass(codeClass);
        }
    }
}
