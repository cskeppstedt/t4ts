using System.CodeDom;
using EnvDTE;
using System;
using System.Linq;

namespace T4TS
{
    public class EnumTraverser
    {
        public CodeEnum CodeEnum { get; private set; }
        public Action<CodeVariable, int> WithVariable { get; set; }

        public EnumTraverser(CodeEnum codeEnum, Action<CodeVariable, int> withVariable)
        {
            if (codeEnum == null)
                throw new ArgumentNullException("codeEnum");

            if (withVariable == null)
                throw new ArgumentNullException("withVariable");

            this.CodeEnum = codeEnum;
            this.WithVariable = withVariable;

            if (codeEnum.Members != null)
                Traverse(codeEnum.Members);
        }

        private void Traverse(CodeElements members)
        {
            var index = 0;
            foreach (var property in members.OfType<CodeVariable>())
            {
                WithVariable(property, index);
                if (property.InitExpression != null)
                    index = Int32.Parse(property.InitExpression.ToString());
                index++;
            }
                
        }
    }
}
