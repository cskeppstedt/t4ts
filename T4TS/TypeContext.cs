using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T4TS.Outputs;

namespace T4TS
{
    public class TypeContext : Outputs.TypeReferenceFactory
    {
        private bool useNativeDates;
        private HashSet<string> referencedTypeNames = new HashSet<string>();

        private IDictionary<string, TypeScriptModule> modulesByName =
            new Dictionary<string, TypeScriptModule>();
        private IDictionary<string, TypeScriptInterface> sourceToInterfaceMap
            = new Dictionary<string, TypeScriptInterface>();
        private IDictionary<string, TypeScriptEnum> sourceToEnumMap
            = new Dictionary<string, TypeScriptEnum>();

        private IDictionary<string, string> sourceNamesToOutputTypeMap;


        public TypeContext(bool useNativeDates)
        {
            this.useNativeDates = useNativeDates;

            this.InitializeTypeMap();
        }
        

        public TypeReference GetTypeReference(
            TypeName sourceType)
        {
            this.referencedTypeNames.Add(sourceType.UniversalName);

            return new TypeReference(
                sourceType,
                this);
        }

        public TypeReference GetLiteralReference(string outputName)
        {
            TypeName sourceType = TypeName.FromLiteral(outputName);
            this.sourceNamesToOutputTypeMap.Add(
                sourceType.UniversalName,
                outputName);

            return this.GetTypeReference(sourceType);
        }

        public TypeScriptInterface GetOrCreateInterface(
            string moduleName,
            TypeName sourceType,
            string outputName,
            out bool created)
        {
            TypeScriptInterface result;
            if (!this.sourceToInterfaceMap.TryGetValue(
                sourceType.UniversalName,
                out result))
            {
                result = new TypeScriptInterface(
                    sourceType,
                    this);

                this.sourceToInterfaceMap.Add(
                    sourceType.UniversalName,
                    result);

                this.sourceNamesToOutputTypeMap.Add(
                    sourceType.UniversalName,
                    this.GenerateOutputName(
                        moduleName,
                        outputName,
                        (sourceType.TypeArguments != null)
                            ? sourceType.TypeArguments.Count
                            : 0));

                created = true;
            }
            else
            {
                created = false;
            }

            bool moduleCreated;
            TypeScriptModule module = this.GetOrCreateModule(
                moduleName,
                out moduleCreated);
            module.Interfaces.Add(result);

            return result;
        }

        public TypeScriptEnum GetOrCreateEnum(
            string moduleName,
            TypeName sourceType,
            string outputName,
            out bool created)
        {
            TypeScriptEnum result;
            if (!this.sourceToEnumMap.TryGetValue(
                sourceType.UniversalName,
                out result))
            {
                result = new TypeScriptEnum(
                    sourceType,
                    this);

                this.sourceToEnumMap.Add(
                    sourceType.UniversalName,
                    result);

                this.sourceNamesToOutputTypeMap.Add(
                    sourceType.UniversalName,
                    this.GenerateOutputName(
                        moduleName,
                        outputName,
                        (sourceType.TypeArguments != null)
                            ? sourceType.TypeArguments.Count
                            : 0));

                created = true;
            }
            else
            {
                created = false;
            }

            bool moduleCreated;
            TypeScriptModule module = this.GetOrCreateModule(
                moduleName,
                out moduleCreated);
            module.Enums.Add(result);

            return result;
        }

        public TypeName ResolveOutputTypeName(TypeName sourceType)
        {
            TypeName result;
            string outputName = this.GetOutputName(sourceType.UniversalName);

            if (outputName == null)
            {
                result = sourceType;
            }
            else if (sourceType.TypeArguments == null
                || !sourceType.TypeArguments.Any())
            {
                result = sourceType.ReplaceUnqualifiedName(outputName);
            }
            else
            {
                result = TypeName.ParseDte(
                    String.Format(
                        outputName,
                        sourceType.TypeArguments
                            .Select(
                                (typeArgument) => this.ResolveOutputTypeName(typeArgument).QualifiedName)
                            .ToArray()));
            }
            return result;
        }

        public IEnumerable<TypeScriptModule> GetModules()
        {
            return this.modulesByName.Values
                .OrderBy((module) => module.QualifiedName);
        }

        public IEnumerable<string> GetReferencedTypeNames()
        {
            return this.referencedTypeNames;
        }

        private string GetOutputName(string sourceTypeName)
        {
            string result;
            this.sourceNamesToOutputTypeMap.TryGetValue(
                sourceTypeName,
                out result);
            return result;
        }

        private string GenerateOutputName(
            string moduleName,
            string typeName,
            int typeArgumentCount)
        {
            StringBuilder builder = new StringBuilder();
            if (moduleName != null)
            {
                builder.Append(moduleName);
                builder.Append(".");
            }
            builder.Append(typeName);
            if (typeArgumentCount > 0)
            {
                builder.Append("<");
                for (int index = 0; index < typeArgumentCount; index++)
                {
                    if (index != 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append("{");
                    builder.Append(index);
                    builder.Append("}");
                }
                builder.Append(">");
            }
            return builder.ToString();
        }

        private TypeScriptModule GetOrCreateModule(
            string name,
            out bool created)
        {
            TypeScriptModule result;
            if (this.modulesByName.TryGetValue(
                name,
                out result))
            {
                created = false;
            }
            else
            {
                result = new TypeScriptModule()
                {
                    QualifiedName = name
                };
                this.modulesByName.Add(
                    name,
                    result);
                created = true;
            }
            return result;
        }

        private void InitializeTypeMap()
        {
            this.sourceNamesToOutputTypeMap = new Dictionary<string, string>()
            {
                { typeof(bool).FullName,            TSOutputTypeNames.Bool },
                { typeof(double).FullName,          TSOutputTypeNames.Number },
                { typeof(float).FullName,           TSOutputTypeNames.Number },
                { typeof(decimal).FullName,         TSOutputTypeNames.Number },
                { typeof(byte).FullName,            TSOutputTypeNames.Number },
                { typeof(sbyte).FullName,           TSOutputTypeNames.Number },
                { typeof(Int16).FullName,           TSOutputTypeNames.Number },
                { typeof(Int32).FullName,           TSOutputTypeNames.Number },
                { typeof(Int64).FullName,           TSOutputTypeNames.Number },
                { typeof(UInt16).FullName,          TSOutputTypeNames.Number },
                { typeof(UInt32).FullName,          TSOutputTypeNames.Number },
                { typeof(UInt64).FullName,          TSOutputTypeNames.Number },
                { typeof(string).FullName,          TSOutputTypeNames.String },
                { typeof(Guid).FullName,            TSOutputTypeNames.String },
                { typeof(DateTime).FullName,        this.useNativeDates ? TSOutputTypeNames.Date : TSOutputTypeNames.String },
                { typeof(DateTimeOffset).FullName,  this.useNativeDates ? TSOutputTypeNames.Date : TSOutputTypeNames.String },
                { typeof(Nullable<>).FullName,      TSOutputTypeNames.Nullable },
                { typeof(IList<>).FullName,         TSOutputTypeNames.Array },
                { typeof(List<>).FullName,          TSOutputTypeNames.Array },
                { typeof(ICollection<>).FullName,   TSOutputTypeNames.Array },
                { typeof(IEnumerable<>).FullName,   TSOutputTypeNames.Array },
                { "[]`1",                           TSOutputTypeNames.Array },
                { typeof(IDictionary<,>).FullName,  TSOutputTypeNames.Dictionary },
                { typeof(Dictionary<,>).FullName,  TSOutputTypeNames.Dictionary }
            };
        }

        private class TSOutputTypeNames
        {
            public const string Bool = "boolean";
            public const string Date = "Date";
            public const string Number = "number";
            public const string String = "string";

            public const string Array = "{0}[]";
            public const string Dictionary = "{{ [name: {0}]: {1}}}";
            public const string Nullable = "{0}?";
        }
    }
}
