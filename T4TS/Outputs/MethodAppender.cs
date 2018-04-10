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
            Settings settings,
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
                        output.Append(",");
                    }
                    else
                    {
                        firstArgument = false;
                    }

                    output.Append(" ");
                    output.Append(argument.Name);
                    output.Append(": ");
                    argumentTypeName = this.TypeContext.ResolveOutputTypeName(method.Type);
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
        }
    }
}
