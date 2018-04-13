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
                OutputSettings settings,
                TypeContext typeContext,
                TypeScriptType containingType,
                TypeReference otherType,
                bool toContainingType,
                bool toCamelCase)
                    : base(
                        settings,
                        typeContext,
                        hasBody: true)
            {
                this.containingType = containingType;
                this.otherType = otherType;
                this.toContainingType = toContainingType;
                this.toCamelCase = toCamelCase;
            }

            protected override void AppendBody(
                StringBuilder output,
                int indent,
                TypeScriptMethod method)
            {
                this.AppendIndentedLine(
                    output,
                    indent,
                    "{");
                
                int bodyIndent = indent + 4;
                
                string toObjectName;
                string fromObjectName;

                if (this.toContainingType)
                {
                    toObjectName = "this";
                    fromObjectName = method.Arguments.First().Name;
                }
                else
                {
                    toObjectName = method.Arguments.Last().Name;
                    fromObjectName = "this";
                }

                if (this.containingType.Parent != null)
                {
                    TypeScriptInterface parentType = this.TypeContext.GetInterface(
                        this.containingType.Parent.SourceType);

                    TypeScriptMethod parentCopyMethod = parentType.Methods.FirstOrDefault(
                        (currentMethod) =>
                        {
                            ChangeCaseCopyMethod.OutputAppender appender = currentMethod.Appender as ChangeCaseCopyMethod.OutputAppender;
                            return (appender != null
                                && appender.toContainingType == this.toContainingType);
                        });

                    if (parentCopyMethod != null)
                    {
                        this.AppendMethodCall(
                            output,
                            bodyIndent,
                            "super",
                            parentCopyMethod.Name,
                            method.Arguments
                                .Select((currentArgument) => currentArgument.Name)
                                .ToList());
                    }
                }

                foreach (TypeScriptMember field in containingType.Fields)
                {
                    this.AppendFieldCopy(
                        output,
                        bodyIndent,
                        method,
                        toObjectName,
                        fromObjectName,
                        field);
                }
                
                this.AppendIndentedLine(
                    output,
                    bodyIndent,
                    String.Format(
                        "return {0};",
                        toObjectName));

                this.AppendIndentedLine(
                    output,
                    indent,
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
                string toObjectName,
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
                    toObjectName,
                    toFieldName,
                    fromObjectName,
                    fromFieldName,
                    parentMethod,
                    field.Type);
            }

            private void AppendFieldValueCopy(
                StringBuilder output,
                int indent,
                string toObjectName,
                string toFieldName,
                string fromObjectName,
                string fromFieldName,
                TypeScriptMethod parentMethod,
                TypeReference fieldType)
            {
                TypeName resolvedFieldType = this.TypeContext.ResolveOutputTypeName(fieldType);
                if (!resolvedFieldType.IsArray)
                {
                    TypeScriptInterface interfaceType = this.TypeContext.GetInterface(fieldType.SourceType);
                    TypeScriptMethod copyMethod = null;
                    if (interfaceType != null
                        && interfaceType.Methods != null)
                    {
                        copyMethod = interfaceType.Methods.FirstOrDefault(
                            (fieldInterfaceMethod) =>
                            {
                                ChangeCaseCopyMethod.OutputAppender appender =
                                    fieldInterfaceMethod.Appender as ChangeCaseCopyMethod.OutputAppender;
                                return (appender != null
                                    && appender.toContainingType == this.toContainingType);
                            });
                    }

                    string rightHandSide;
                    if (copyMethod != null)
                    {
                        rightHandSide = this.GetMethodCall(
                            toObjectName,
                            fromObjectName,
                            fromFieldName,
                            interfaceType,
                            copyMethod);
                    }
                    else
                    {
                        rightHandSide = String.Format(
                            "{0}.{1}",
                            fromObjectName,
                            fromFieldName);
                    }

                    this.AppendIndentedLine(
                        output,
                        indent,
                        String.Format(
                            "{0}.{1} = {2};",
                            toObjectName,
                            toFieldName,
                            rightHandSide));
                }
                else
                {
                    this.AppendArrayCopy(
                        output,
                        indent,
                        toObjectName,
                        toFieldName,
                        fromObjectName,
                        fromFieldName,
                        parentMethod,
                        fieldType);
                }
            }

            private string GetMethodCall(
                string toObjectName,
                string fromObjectName,
                string fromFieldName,
                TypeScriptInterface interfaceType,
                TypeScriptMethod copyMethod)
            {
                string result;
                if (this.toContainingType)
                {
                    TypeName outputName = this.TypeContext.ResolveOutputTypeName(interfaceType);
                    result = String.Format(
                        "new {0}().{1}({2}.{3})",
                        outputName.QualifiedName,
                        copyMethod.Name,
                        fromObjectName,
                        fromFieldName);
                }
                else
                {
                    TypeName outputName = this.TypeContext.ResolveOutputTypeName(copyMethod.Arguments.First().Type);
                    result = String.Format(
                        "{0}.{1}.{2}(new {3}())",
                        fromObjectName,
                        fromFieldName,
                        copyMethod.Name,
                        outputName.QualifiedName);
                }
                return result;
            }


            private void AppendArrayCopy(
                StringBuilder output,
                int indent,
                string toObjectName,
                string toFieldName,
                string fromObjectName,
                string fromFieldName,
                TypeScriptMethod parentMethod,
                TypeReference fieldType)
            {
                TypeName resolvedType = this.TypeContext.ResolveOutputTypeName(fieldType);
                TypeName itemType = fieldType.SourceType.TypeArguments.First();
                TypeName resolvedItemType = this.TypeContext.ResolveOutputTypeName(
                    this.TypeContext.GetTypeReference(
                        itemType,
                        fieldType.ContextTypeReference));

                this.AppendIndentedLine(
                    output,
                    indent,
                    String.Format(
                        "{0}.{1} = new Array({3}.{4}.length);",
                        toObjectName,
                        toFieldName,
                        resolvedItemType.QualifiedName,
                        fromObjectName,
                        fromFieldName));

                this.AppendIndentedLine(
                    output,
                    indent,
                    String.Format(
                        "for (var index{0}: number = 0; index{0} < {1}.{2}.length; index{0}++)",
                        indent / 4,
                        fromObjectName,
                        fromFieldName));

                this.AppendIndentedLine(
                    output,
                    indent,
                    "{");

                this.AppendFieldValueCopy(
                    output,
                    indent + 4,
                    toObjectName,
                    String.Format(
                        "{0}[index{1}]",
                        toFieldName,
                        indent / 4),
                    fromObjectName,
                    String.Format(
                        "{0}[index{1}]",
                        fromFieldName,
                        indent / 4),
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
