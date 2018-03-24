using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace T4TS
{
    public class TypeContext
    {
        private bool useNativeDates;

        private IDictionary<Type, IDictionary<string, TypeScriptOutputType>> outputsByTypeAndName
            = new Dictionary<Type, IDictionary<string, TypeScriptOutputType>>();
        private IDictionary<string, TypeScriptModule> modulesByName =
            new Dictionary<string, TypeScriptModule>();
        private IList<TypeScriptDelayResolveType> delayResolveTypes
            = new List<TypeScriptDelayResolveType>();

        public TypeContext(bool useNativeDates)
        {
            this.useNativeDates = useNativeDates;
        }

        private static readonly string[] genericCollectionTypeStarts = new string[] {
            "System.Collections.Generic.List<",
            "System.Collections.Generic.IList<",
            "System.Collections.Generic.ICollection<",
            "System.Collections.Generic.IEnumerable<"
        };

        private static readonly string nullableTypeStart = "System.Nullable<";
        
        public TypeScriptInterface GetOrCreateInterface(
            string moduleName,
            TypeName typeName,
            out bool created)
        {
            TypeScriptInterface result = this.GetOrCreateOutput<TypeScriptInterface>(
                typeName.UniversalName,
                out created);
            if (created)
            {
                result.SourceType = typeName;
            }

            bool moduleCreated;
            TypeScriptModule module = this.GetOrCreateModule(
                moduleName,
                out moduleCreated);
            if (result.Module == null)
            {
                module.Interfaces.Add(result);
                result.Module = module;
            }
            else if (result.Module.QualifiedName != moduleName)
            {
                throw new System.InvalidOperationException(
                    "Interface was registered with multiple module names " + typeName.UniversalName);
            }
            return result;
        }

        public TypeScriptEnum GetOrCreateEnum(
            string moduleName,
            string fullName,
            out bool created)
        {
            TypeScriptEnum result = this.GetOrCreateOutput<TypeScriptEnum>(
                fullName,
                out created);

            bool moduleCreated;
            TypeScriptModule module = this.GetOrCreateModule(
                moduleName,
                out moduleCreated);
            if (result.Module == null)
            {
                module.Enums.Add(result);
                result.Module = module;
            }
            else if (result.Module.QualifiedName != moduleName)
            {
                throw new System.InvalidOperationException(
                    "Interface was registered with multiple module names " + fullName);
            }
            return result;
        }

        public TType GetOrCreateOutput<TType>(
            string universalName,
            out bool created)
                where TType : TypeScriptOutputType, new()
        {
            IDictionary<string, TypeScriptOutputType> outputsByName;
            if (!this.outputsByTypeAndName.TryGetValue(
                typeof(TType),
                out outputsByName))
            {
                outputsByName = new Dictionary<string, TypeScriptOutputType>();
                this.outputsByTypeAndName.Add(
                    typeof(TType),
                    outputsByName);
            }

            TypeScriptOutputType result;
            if (outputsByName.TryGetValue(
                universalName,
                out result))
            {
                created = false;
            }
            else
            {
                result = new TType()
                {
                    FullName = universalName
                };
                outputsByName.Add(
                    universalName,
                    result);
                created = true;
            }
            return (TType)result;
        }
        
        public TypeScriptOutputType GetOutput(TypeName typeName)
        {
            TypeScriptOutputType result = null;
            foreach (IDictionary<string, TypeScriptOutputType> outputsByName in this.outputsByTypeAndName.Values)
            {
                if (outputsByName.TryGetValue(
                    typeName.UniversalName,
                    out result))
                {
                    break;
                }
            }
            return result;
        }

        public TypeScriptOutputType GetResolvableTypeFromDteName(string fullName)
        {
            TypeScriptOutputType result = this.GetSimpleSystemOutputType(fullName);
            if (result == null)
            {
                TypeScriptDelayResolveType delayType = new TypeScriptDelayResolveType()
                {
                    TypeContext = this,
                    SourceType = TypeName.ParseDte(fullName)
                };

                this.delayResolveTypes.Add(delayType);

                result = delayType;
            }
            return result;
        }

        public TypeScriptOutputType GetSystemOutputType(TypeName typeName)
        {
            TypeScriptOutputType result;

            TypescriptType systemType = this.GetSimpleSystemType(typeName.QualifiedName);
            if (systemType == null)
            {
                systemType = this.GetComplexSystemType(typeName.QualifiedName);
            }

            string literal = null;
            if (systemType != null)
            {
                literal = systemType.ToString();
            }
            else if (String.IsNullOrEmpty(typeName.Namespace))
            {
                literal = typeName.RawName;
            }

            if (literal != null)
            {
                result = new TypeScriptLiteralType()
                {
                    FullName = literal
                };
            }
            else
            {
                result = null;
            }
            return result;
        }
        
        public IEnumerable<TypeScriptModule> GetModules()
        {
            return this.modulesByName.Values
                .OrderBy((module) => module.QualifiedName);
        }

        public IEnumerable<TypeScriptDelayResolveType> GetDelayResolveTypes()
        {
            return this.delayResolveTypes;
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


        private TypescriptType ResolveType(string rawName)
        {
            TypeName typeName = TypeName.ParseDte(rawName);
            TypeScriptOutputType outputType = this.GetOutput(typeName);
            if (outputType == null)
            {
                outputType = this.GetSystemOutputType(typeName);
            }
            return new OutputType(outputType);
        }

        private TypescriptType GetComplexSystemType(string typeFullName)
        {
            TypescriptType result = null;
            if (IsGenericEnumerable(typeFullName))
            {
                return new ArrayType
                {
                    ElementType = ResolveType(UnwrapGenericType(typeFullName))
                };
            }
            else if (IsNullable(typeFullName))
            {
                return new NullableType
                {
                    WrappedType = ResolveType(UnwrapGenericType(typeFullName))
                };
            }

            var realType = TypeFullNameParser.Parse(typeFullName);

            if (realType.IsEnumerable()
                || realType.IsArray())
            {
                return new ArrayType()
                {
                    ElementType = ResolveType(realType.TypeArgumentFullNames[0].FullName)
                };
            }
            else if (realType.IsDictionary())
            {
                return new DictionaryType()
                {
                    KeyType = ResolveType(realType.TypeArgumentFullNames[0].FullName),
                    ElementType = ResolveType(realType.TypeArgumentFullNames[1].FullName)
                };
            }
            else if (TypeFullName.IsGenericType(typeFullName))
            {
                return new GenericType()
                {
                    BaseType = this.ResolveType(realType.FullName),
                    TypeArguments = realType.TypeArgumentFullNames
                        .Select((typeArgument) => this.ResolveType(typeArgument.FullName))
                        .ToList()
                };
            }
            return result;
        }

        private TypeScriptOutputType GetSimpleSystemOutputType(string typeFullName)
        {
            TypescriptType simpleType = this.GetSimpleSystemType(typeFullName);
            return (simpleType != null)
                ? new TypeScriptLiteralType()
                {
                    FullName = simpleType.ToString()
                }
                : null;
        }

        private TypescriptType GetSimpleSystemType(string typeFullName)
        {
            TypescriptType result = null;
            switch (typeFullName)
            {
                case "System.Guid":
                    result = new GuidType();
                    break;
                case "System.Boolean":
                    result = new BoolType();
                    break;
                case "System.Double":
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                case "System.Decimal":
                case "System.Byte":
                case "System.SByte":
                case "System.Single":
                    result = new NumberType();
                    break;

                case "System.String":
                    result = new StringType();
                    break;

                case "System.DateTime":
                case "System.DateTimeOffset":
                    if (this.useNativeDates)
                        result = new DateTimeType();
                    else
                        result = new StringType();
                    break;
            }
            return result;
        }

        private bool IsNullable(string typeFullName)
        {
            return typeFullName.StartsWith(nullableTypeStart);
        }

        public string UnwrapGenericType(string typeFullName)
        {
            int firstIndex = typeFullName.IndexOf('<');
            return typeFullName.Substring(firstIndex+1, typeFullName.Length - firstIndex- 2);
        }

        public bool IsGenericEnumerable(string typeFullName)
        {
            return genericCollectionTypeStarts.Any(t => typeFullName.StartsWith(t));
        }
    }
}
