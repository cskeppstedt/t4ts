using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs.Custom
{
    public partial class ChangeCaseCopyMethod
    {
        protected class OutputAppender : MethodAppender
        {
            private TypeScriptType containingType;
            private TypeReference otherType;
            private bool toContainingType;
            private bool toCamelCase;

            public OutputAppender(
                Settings settings,
                TypeContext typeContext,
                TypeScriptType containingType,
                TypeReference otherType,
                bool toContainingType,
                bool toCamelCase)
                    : base(
                        settings,
                        typeContext)
            {
                this.containingType = containingType;
                this.otherType = otherType;
                this.toContainingType = toContainingType;
                this.toCamelCase = toCamelCase;
            }

            protected override void AppendBody(
                StringBuilder output,
                int baseIndentation,
                TypeScriptMethod method)
            {
                this.AppendIndentedLine(
                    output,
                    baseIndentation,
                    "{");

                TypeName returnType = this.TypeContext.ResolveOutputTypeName(method.Type);

                int bodyIndent = baseIndentation + 4;
                this.AppendIndentedLine(
                    output,
                    bodyIndent,
                    String.Format(
                        "{0} result = new {0}();",
                        this.otherType.SourceType.QualifiedName));

                string fromObjectName = (this.toContainingType)
                    ? method.Arguments.First().Name
                    : "this";
                
                foreach (TypeScriptMember field in containingType.Fields)
                {
                    this.AppendFieldCopy(
                        output,
                        bodyIndent,
                        method,
                        fromObjectName,
                        field);
                }

                this.AppendIndentedLine(
                    output,
                    bodyIndent,
                    "return result;");

                this.AppendIndentedLine(
                    output,
                    baseIndentation,
                    "}");
            }

            private void AppendFieldCopy(
                StringBuilder output,
                int indent,
                TypeScriptMethod parentMethod,
                string fromObjectName,
                TypeScriptMember field)
            {
                string camelCase = field.Name[0].ToString().ToLower()
                    + field.Name.Substring(1);
                string pascalCase = field.Name[0].ToString().ToUpper()
                    + field.Name.Substring(1);

                string fromName;
                string toName;
                if (this.toCamelCase)
                {
                    toName = camelCase;
                    fromName = pascalCase;
                }
                else
                {
                    toName = pascalCase;
                    fromName = camelCase;
                }

                TypeScriptInterface interfaceType = this.TypeContext.GetInterface(field.Type.SourceType);
                TypeScriptMethod copyMethod = null;
                if (interfaceType != null
                    && interfaceType.Methods != null)
                {
                    copyMethod = interfaceType.Methods.FirstOrDefault(
                        (fieldInterfaceMethod)
                            => fieldInterfaceMethod.Appender is ChangeCaseCopyMethod.OutputAppender
                                && fieldInterfaceMethod.Name == parentMethod.Name);
                }

                if (copyMethod != null)
                {
                    this.AppendIndentedLine(
                        output,
                        indent,
                        String.Format(
                            "result.{0} = {1};",
                            toName,
                            this.GetMethodCall(
                                fromObjectName,
                                fromName,
                                interfaceType,
                                copyMethod)));
                }
                else
                {
                    this.AppendIndentedLine(
                        output,
                        indent,
                        String.Format(
                            "result.{0} = {1}.{2};",
                            toName,
                            fromObjectName,
                            fromName));
                }
            }

            private string GetMethodCall(
                string fromObjectName,
                string fromName,
                TypeScriptInterface interfaceType,
                TypeScriptMethod copyMethod)
            {
                string result;
                if (copyMethod.IsStatic)
                {
                    TypeName outputName = this.TypeContext.ResolveOutputTypeName(interfaceType);
                    result = String.Format(
                        "{0}.{1}({2}.{3})",
                        outputName.QualifiedName,
                        copyMethod.Name,
                        fromObjectName,
                        fromName);
                }
                else
                {
                    result = String.Format(
                        "{0}.{1}()",
                        fromObjectName,
                        copyMethod.Name);
                }
                return result;
            }
        }
    }
}
