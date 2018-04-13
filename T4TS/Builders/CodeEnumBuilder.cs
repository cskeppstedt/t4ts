using EnvDTE;
using T4TS.Outputs;

namespace T4TS.Builders
{
    public interface CodeEnumBuilder
    {
        TypeScriptEnum Build(
            CodeEnum codeEnum,
            TypeContext typeContext);
    }
}
