using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public class MethodAppender : OutputAppender<TypeScriptMethod>
    {
        private bool hasBody;

        public MethodAppender(
            OutputSettings settings,
            TypeContext typeContext,
            bool hasBody)
                : base(
                    settings,
                    typeContext)
        {
            this.hasBody = hasBody;
        }

        public override void AppendOutput(
            StringBuilder output,
            int indent,
            TypeScriptMethod method)
        {
            this.AppendMethodPrototype(
                output,
                indent,
                method);

            if (this.hasBody)
            {
                output.AppendLine();

                this.AppendBody(
                    output,
                    indent,
                    method);
            }
            else
            {
                output.AppendLine(";");
            }
        }

        protected virtual void AppendBody(
            StringBuilder output,
            int indent,
            TypeScriptMethod method)
        {
            this.AppendIndentedLine(
                output,
                indent,
                "{");

            TypeScriptConstructor constructor = method as TypeScriptConstructor;
            if (constructor != null)
            {
                output.AppendLine();

                this.AppendMethodCall(
                    output,
                    indent + 4,
                    objectName: null,
                    methodName: "super",
                    methodArguments: constructor.BaseArguments);
            }

            this.AppendIndentedLine(
                output,
                indent,
                "}");
        }

        protected void AppendMethodCall(
            StringBuilder output,
            int indent,
            string objectName,
            string methodName,
            IList<string> methodArguments)
        {
            IList<string> baseArguments = methodArguments ?? new List<string>(0);

            this.AppendIndendation(
                output,
                indent);

            if (!String.IsNullOrEmpty(objectName))
            {
                output.Append(objectName);
                output.Append('.');
            }

            output.AppendLine(String.Format(
                "{0}({1});",
                methodName,
                String.Join(
                    ", ",
                    baseArguments)));
        }

        protected void AppendMethodPrototype(
            StringBuilder output,
            int indent,
            TypeScriptMethod method)
        {
            this.AppendIndendation(
                output,
                indent);

            if (method.IsStatic)
            {
                output.Append("static ");
            }

            output.Append(method.Name);

            if (method.TypeArguments != null
                && method.TypeArguments.Any())
            {
                output.Append("<");

                bool firstArgument = true;

                TypeName resolvedName;
                foreach (TypeReference argumentReference in method.TypeArguments)
                {
                    if (!firstArgument)
                    {
                        output.Append(", ");
                    }
                    else
                    {
                        firstArgument = false;
                    }

                    resolvedName = this.TypeContext.ResolveOutputTypeName(argumentReference);
                    output.Append(resolvedName.QualifiedName);
                }

                output.Append(">");
            }

            output.Append("(");

            if (method.Arguments != null
                && method.Arguments.Any())
            {
                bool firstArgument = true;
                TypeName argumentTypeName;
                foreach (TypeScriptMember argument in method.Arguments)
                {
                    if (!firstArgument)
                    {
                        output.Append(", ");
                    }
                    else
                    {
                        firstArgument = false;
                    }

                    output.Append(argument.Name);
                    output.Append(": ");
                    argumentTypeName = this.TypeContext.ResolveOutputTypeName(argument.Type);
                    output.Append(argumentTypeName.QualifiedName);
                }
            }
            output.Append(")");

            if (method.Type != null)
            {
                TypeName returnTypeName = this.TypeContext.ResolveOutputTypeName(method.Type);
                output.Append(": " + returnTypeName.QualifiedName);
            }
        }
    }
}
