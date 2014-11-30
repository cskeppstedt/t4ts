using System;
using System.Linq;
using EnvDTE;

namespace T4TS
{
    public class EnumTraverser
    {
        public EnumTraverser(CodeEnum codeEnum, Action<CodeVariable, int> withVariable)
        {
            if (codeEnum == null) throw new ArgumentNullException("codeEnum");

            if (withVariable == null) throw new ArgumentNullException("withVariable");

            CodeEnum = codeEnum;
            WithVariable = withVariable;

            if (codeEnum.Members != null)
                Traverse(codeEnum.Members);
        }

        public CodeEnum CodeEnum { get; private set; }
        public Action<CodeVariable, int> WithVariable { get; set; }

        private void Traverse(CodeElements members)
        {
            int index = 0;
            foreach (CodeVariable property in members.OfType<CodeVariable>())
            {
                WithVariable(property, index);
                if (property.InitExpression != null)
                    index = Int32.Parse(property.InitExpression.ToString());
                index++;
            }
        }
    }
}