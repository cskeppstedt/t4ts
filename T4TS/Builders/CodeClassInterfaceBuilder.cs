using EnvDTE;

namespace T4TS.Builders
{
    public interface CodeClassInterfaceBuilder
    {
        TypeScriptInterface Build(
            CodeClass codeClass,
            TypeContext typeContext);
    }
}
