using EnvDTE;
using System;
using System.Linq;

namespace T4TS
{
    public class ClassTraverser
    {
        public CodeClass CodeClass { get; private set; }
        public Action<CodeProperty> WithProperty { get; set; }

        public ClassTraverser(CodeClass codeClass, Action<CodeProperty> withProperty)
        {
            if (codeClass == null)
                throw new ArgumentNullException("codeClass");
            
            if (withProperty == null)
                throw new ArgumentNullException("withProperty");

            this.CodeClass = codeClass;
            this.WithProperty = withProperty;

            if (codeClass.Members != null)
                Traverse(codeClass.Members);
        }

        private void Traverse(CodeElements members)
        {
            foreach (var property in members)
            {
                if (property is CodeProperty)
                    WithProperty((CodeProperty)property);
            }
        }
    }
}
