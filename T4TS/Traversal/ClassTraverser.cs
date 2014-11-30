﻿using System;
using System.Linq;
using EnvDTE;

namespace T4TS
{
    public class ClassTraverser
    {
        public ClassTraverser(CodeClass codeClass, Action<CodeProperty> withProperty)
        {
            if (codeClass == null) throw new ArgumentNullException("codeClass");
            if (withProperty == null) throw new ArgumentNullException("withProperty");

            CodeClass = codeClass;
            WithProperty = withProperty;

            if (codeClass.Members != null)
                Traverse(codeClass.Members);
        }

        public CodeClass CodeClass { get; private set; }
        public Action<CodeProperty> WithProperty { get; private set; }

        private void Traverse(CodeElements members)
        {
            foreach (CodeProperty property in members.OfType<CodeProperty>())
                WithProperty(property);
        }
    }
}