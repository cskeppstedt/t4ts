using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public abstract class MethodAppender : OutputAppender<TypeScriptMethod>
    {
        public MethodAppender(
            OutputSettings settings,
            TypeContext typeContext)
                : base(
                    settings,
                    typeContext)
        {
        }

        public override void AppendOutput(
            StringBuilder output,
            int baseIndentation,
            TypeScriptMethod method)
        {
            this.AppendMethodPrototype(
                output,
                baseIndentation,
                method);

            this.AppendBody(
                output,
                baseIndentation,
                method);
        }

        protected abstract void AppendBody(
            StringBuilder output,
            int baseIndentation,
            TypeScriptMethod method);

        protected void AppendMethodPrototype(
            StringBuilder output,
            int baseIndentation,
            TypeScriptMethod method)
        {
            this.AppendIndendation(
                output,
                baseIndentation);

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
            output.AppendLine();

            TypeScriptConstructor constructor = method as TypeScriptConstructor;
            if (constructor != null
                && constructor.BaseArguments != null
                && constructor.BaseArguments.Any())
            {
                this.AppendIndentedLine(
                    output,
                    baseIndentation + 4,
                    String.Format(
                        ": base({0})",
                        String.Join(
                            ", ",
                            constructor.BaseArguments)));
            }
        }
    }
}
