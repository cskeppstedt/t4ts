using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using T4TS.Outputs;

namespace T4TS
{
    public partial class TypeContext
    {
        private Settings settings;

        private IList<TypeReference> typeReferences = new List<TypeReference>();

        private IDictionary<string, TypeScriptModule> modulesByName =
            new Dictionary<string, TypeScriptModule>();
        private IDictionary<string, TypeScriptInterface> sourceToInterfaceMap
            = new Dictionary<string, TypeScriptInterface>();
        private IDictionary<string, TypeScriptEnum> sourceToEnumMap
            = new Dictionary<string, TypeScriptEnum>();

        private IDictionary<string, string> sourceNamesToOutputTypeMap;


        public TypeContext()
            : this(new Settings())
        {
        }

        public TypeContext(Settings settings)
        {
            this.settings = settings;

            this.InitializeTypeMap();
        }
        

        public TypeReference GetTypeReference(
            TypeName sourceType,
            TypeReference contextTypeReference)
        {
            TypeReference result = new TypeReference(
                sourceType,
                this.GetTypeArgumentReferences(
                    sourceType.TypeArguments,
                    contextTypeReference),
                contextTypeReference);

            this.typeReferences.Add(result);
            return result;
        }

        public TypeReference GetLiteralReference(string outputName)
        {
            TypeName sourceType = TypeName.FromLiteral(outputName);

            string currentName;
            if (this.sourceNamesToOutputTypeMap.TryGetValue(
                outputName,
                out currentName))
            {
                if (currentName != outputName)
                {
                    throw new InvalidOperationException(String.Format(
                        "Trying to map name {0} when it's already maps to {1}",
                        outputName, 
                        currentName));
                }
            }
            else
            { 
                this.sourceNamesToOutputTypeMap.Add(
                    sourceType.UniversalName,
                    outputName);
            }

            return this.GetTypeReference(
                sourceType,
                contextTypeReference: null);
        }

        public TypeScriptInterface GetInterface(
            TypeName sourceType)
        {
            TypeScriptInterface result;
            this.sourceToInterfaceMap.TryGetValue(
                sourceType.UniversalName,
                out result);
            return result;
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
                    this.GetTypeArgumentReferences(
                        sourceType.TypeArguments,
                        contextTypeReference: null),
                    contextTypeReference: null);

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

                bool moduleCreated;
                TypeScriptModule module = this.GetOrCreateModule(
                    moduleName,
                    out moduleCreated);
                module.Interfaces.Add(result);

                created = true;
            }
            else
            {
                created = false;
            }

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
                    this.GetTypeArgumentReferences(
                        sourceType.TypeArguments,
                        contextTypeReference: null),
                    contextTypeReference: null);

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

        public TypeName ResolveOutputTypeName(TypeReference typeReference)
        {
            return this.ResolveOutputTypeName(
                typeReference,
                allowNull: false);
        }

        public TypeName ResolveOutputTypeName(TypeReference typeReference, bool allowNull)
        {
            TypeName result;
            string outputName = this.GetOutputName(typeReference.SourceType.UniversalName);

            if (outputName == null)
            {
                if (typeReference.ContextTypeReference != null
                    && typeReference.ContextTypeReference.SourceType.TypeArguments != null)
                {
                    result = typeReference.ContextTypeReference.SourceType.TypeArguments.FirstOrDefault(
                        (typeName) => typeName.QualifiedName == typeReference.SourceType.QualifiedName);
                }
                else
                {
                    result = null;
                }
                
                if (result == null
                    && !allowNull)
                {
                    result = TypeName.FromLiteral("UNKNOWN_TYPE_" + typeReference.SourceType.RawName);
                }
            }
            else if (typeReference.SourceType.TypeArguments == null
                || !typeReference.SourceType.TypeArguments.Any())
            {
                result = typeReference.SourceType.ReplaceUnqualifiedName(outputName);
            }
            else if (typeReference is TypeScriptInterface)
            {
                result = TypeName.Format(
                    outputName,
                    typeReference.SourceType.TypeArguments);
            }
            else
            {
                IEnumerable<string> resolvedTypes = typeReference.TypeArgumentReferences
                    .Select(
                        (typeArgumentReference) =>
                        {
                            TypeName argumentName = this.ResolveOutputTypeName(typeArgumentReference, allowNull);
                            return (argumentName != null)
                                ? argumentName.QualifiedName
                                : typeArgumentReference.SourceType.QualifiedName;
                        });
                result = TypeName.Format(
                    outputName,
                    resolvedTypes);
            }
            return result;
        }


        public IEnumerable<TypeScriptModule> GetModules()
        {
            return this.modulesByName.Values
                .OrderBy((module) => module.QualifiedName);
        }

        public IEnumerable<TypeReference> GetTypeReferences()
        {
            return this.typeReferences;
        }

        public void SetOutputName(
            string sourceName,
            string outputName)
        {
            this.sourceNamesToOutputTypeMap[sourceName] = outputName;
        }

        private string GetOutputName(string sourceTypeName)
        {
            string result;
            this.sourceNamesToOutputTypeMap.TryGetValue(
                sourceTypeName,
                out result);
            return result;
        }

        private IList<TypeReference> GetTypeArgumentReferences(
            IEnumerable<TypeName> typeArguments,
            TypeReference contextTypeReference)
        {
            IList<TypeReference> result;
            if (typeArguments == null
                || !typeArguments.Any())
            {
                result = new List<TypeReference>();
            }
            else
            {
                result = typeArguments
                    .Select((typeArgument) =>
                        this.GetTypeReference(
                            typeArgument,
                            contextTypeReference))
                    .ToList();
            }
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
                {
                    typeof(bool).FullName,
                    (this.settings.CompatibilityVersion < TSOutputTypeNames.OldBoolEndVersion)
                        ? TSOutputTypeNames.OldBool
                        : TSOutputTypeNames.Bool
                },
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
                {
                    typeof(DateTime).FullName,
                    (this.settings.UseNativeDates)
                        ? TSOutputTypeNames.Date
                        : TSOutputTypeNames.String
                },
                {
                    typeof(DateTimeOffset).FullName,
                    this.settings.UseNativeDates
                        ? TSOutputTypeNames.Date
                        : TSOutputTypeNames.String
                },
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
            // All types are nullable in TS, no markup needed
            public const string Nullable = "{0}";

            public const string OldBool = "bool";
            public static readonly Version OldBoolEndVersion = new Version(0, 9, 0);
        }
    }
}
