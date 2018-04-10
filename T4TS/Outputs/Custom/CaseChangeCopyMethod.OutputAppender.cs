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

                string camelCase;
                string pascalCase;
                string fromName;
                string toName;
                foreach (TypeScriptMember field in containingType.Fields)
                {
                    camelCase = field.Name[0].ToString().ToLower()
                        + field.Name.Substring(1);
                    pascalCase = field.Name[0].ToString().ToUpper()
                        + field.Name.Substring(1);
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

                    this.AppendIndentedLine(
                        output,
                        bodyIndent,
                        String.Format(
                            "result.{0} = {1}.{2};",
                            toName,
                            fromObjectName,
                            fromName));
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
        }
    }
}
