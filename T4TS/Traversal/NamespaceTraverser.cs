using System;
using System.Linq;
using EnvDTE;

namespace T4TS
{
    public class NamespaceTraverser
    {
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

        public Action<CodeClass> WithCodeClass { get; private set; }
        public Action<CodeEnum> WithCodeEnum { get; private set; }

        private void Traverse(CodeElements members)
        {
            foreach (CodeEnum codeEnum in members.OfType<CodeEnum>())
                WithCodeEnum(codeEnum);
            foreach (CodeClass codeClass in members.OfType<CodeClass>())
                WithCodeClass(codeClass);
        }
    }
}