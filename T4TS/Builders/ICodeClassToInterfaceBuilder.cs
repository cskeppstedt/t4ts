using EnvDTE;
using T4TS.Outputs;

namespace T4TS.Builders
{
    public interface ICodeClassToInterfaceBuilder
    {
        TypeScriptInterface Build(
            CodeClass codeClass,
            TypeContext typeContext);
    }
}
