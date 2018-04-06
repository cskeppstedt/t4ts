using EnvDTE;
using T4TS.Outputs;

namespace T4TS.Builders
{
    public interface ICodeInterfaceToInterfaceBuilder
    {
        TypeScriptInterface Build(
            CodeInterface codeInterface,
            TypeContext typeContext);
    }
}
