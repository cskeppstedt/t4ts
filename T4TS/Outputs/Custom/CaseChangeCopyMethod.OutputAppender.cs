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

            private void DetermineFieldNames(
                string baseName,
                out string toFieldName,
                out string fromFieldName)
            {
                string allExceptFirst = baseName.Substring(1);
                string camelCase = baseName[0].ToString().ToLower() + allExceptFirst;
                string pascalCase = baseName[0].ToString().ToUpper() + allExceptFirst;
                
                if (this.toCamelCase)
                {
                    toFieldName = camelCase;
                    fromFieldName = pascalCase;
                }
                else
                {
                    toFieldName = pascalCase;
                    fromFieldName = camelCase;
                }
            }

            private void AppendFieldCopy(
                StringBuilder output,
                int indent,
                TypeScriptMethod parentMethod,
                string fromObjectName,
                TypeScriptMember field)
            {
                string toFieldName;
                string fromFieldName;
                this.DetermineFieldNames(
                    field.Name,
                    out toFieldName,
                    out fromFieldName);

                this.AppendFieldValueCopy(
                    output,
                    indent,
                    toFieldName,
                    fromObjectName,
                    fromFieldName,
                    parentMethod,
                    field.Type);
            }

            private void AppendFieldValueCopy(
                StringBuilder output,
                int indent,
                string toName,
                string fromObjectName,
                string fromName,
                TypeScriptMethod parentMethod,
                TypeReference fieldType)
            {
                if (!fieldType.SourceType.IsArray)
                {
                    TypeScriptInterface interfaceType = this.TypeContext.GetInterface(fieldType.SourceType);
                    TypeScriptMethod copyMethod = null;
                    if (interfaceType != null
                        && interfaceType.Methods != null)
                    {
                        copyMethod = interfaceType.Methods.FirstOrDefault(
                            (fieldInterfaceMethod)
                                => fieldInterfaceMethod.Appender is ChangeCaseCopyMethod.OutputAppender
                                    && fieldInterfaceMethod.Name == parentMethod.Name);
                    }

                    string rightHandSide;
                    if (copyMethod != null)
                    {
                        rightHandSide = this.GetMethodCall(
                            fromObjectName,
                            fromName,
                            interfaceType,
                            copyMethod);
                    }
                    else
                    {
                        rightHandSide = String.Format(
                            "{0}.{1}",
                            fromObjectName,
                            fromName);
                    }

                    this.AppendIndentedLine(
                        output,
                        indent,
                        String.Format(
                            "result.{0} = {1};",
                            toName,
                            rightHandSide));
                }
                else
                {
                    this.AppendArrayCopy(
                        output,
                        indent,
                        toName,
                        fromObjectName,
                        fromName,
                        parentMethod,
                        fieldType);
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


            private void AppendArrayCopy(
                StringBuilder output,
                int indent,
                string toName,
                string fromObjectName,
                string fromName,
                TypeScriptMethod parentMethod,
                TypeReference fieldType)
            {
                TypeName resolvedType = this.TypeContext.ResolveOutputTypeName(fieldType);
                TypeName itemType = fieldType.SourceType.TypeArguments.First();
                this.AppendIndentedLine(
                    output,
                    indent,
                    String.Format(
                        "result.{0} = new {1}[{2}.{3}.length];",
                        toName,
                        itemType,
                        fromObjectName,
                        fromName));

                this.AppendIndentedLine(
                    output,
                    indent,
                    String.Format(
                        "for (var index{0}: number = 0; index{0} < {1}.{2}.length; index{0}++)",
                        indent / 4,
                        fromObjectName,
                        fromName));

                this.AppendIndentedLine(
                    output,
                    indent,
                    "{");

                this.AppendFieldValueCopy(
                    output,
                    indent + 4,
                    String.Format(
                        "{0}[index{1}]",
                        toName,
                        indent / 4),
                    fromObjectName,
                    fromName,
                    parentMethod,
                    this.TypeContext.GetTypeReference(
                        itemType,
                        fieldType.ContextTypeReference));

                this.AppendIndentedLine(
                    output,
                    indent,
                    "}");
            }
        }
    }
}
