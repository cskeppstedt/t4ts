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
        private IDictionary<string, TypeScriptDelayResolveType> delayLoadByName
            = new Dictionary<string, TypeScriptDelayResolveType>();

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
            string fullName,
            out bool created)
        {
            TypeScriptInterface result = this.GetOrCreateOutput<TypeScriptInterface>(
                fullName,
                out created);

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
                    "Interface was registered with multiple module names " + fullName);
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

        public TypeScriptOutputType GetOrCreateOutputType(
            string fullName,
            bool resolveOutputOnly,
            out bool created)
        {
            TypeScriptOutputType result = this.GetOutput(fullName);
            if (result != null)
            {
                created = false;
            }
            else
            {
                TypeScriptDelayResolveType delayType;
                if (this.delayLoadByName.TryGetValue(
                    fullName,
                    out delayType))
                {
                    created = true;
                }
                else
                {
                    delayType = new TypeScriptDelayResolveType(
                        this,
                        resolveOutputOnly)
                    {
                        FullName = fullName
                    };
                    this.delayLoadByName.Add(
                        fullName,
                        delayType);
                    created = true;
                }
                result = delayType;
            }
            return result;
        }

        public TType GetOrCreateOutput<TType>(
            string fullName,
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
                fullName,
                out result))
            {
                created = false;
            }
            else
            {
                result = new TType()
                {
                    FullName = fullName
                };
                outputsByName.Add(
                    fullName,
                    result);
                created = true;
            }
            return (TType)result;
        }
        
        public TypeScriptOutputType GetOutput(string fullName)
        {
            TypeScriptOutputType result = null;
            if (fullName != null)
            {
                foreach (IDictionary<string, TypeScriptOutputType> outputsByName in this.outputsByTypeAndName.Values)
                {
                    if (outputsByName.TryGetValue(
                      fullName,
                      out result))
                    {
                        break;
                    }
                }
            }
            return result;
        }

        public IEnumerable<TypeScriptModule> GetModules()
        {
            return this.modulesByName.Values
                .OrderBy((module) => module.QualifiedName);
        }

        public IEnumerable<TypeScriptDelayResolveType> GetDelayLoadTypes()
        {
            return this.delayLoadByName.Values
                .OrderBy((module) => module.FullName);
        }

        public TypescriptType GetTypeScriptType(CodeTypeRef codeType)
        {
            switch (codeType.TypeKind)
            {
                case vsCMTypeRef.vsCMTypeRefChar:
                case vsCMTypeRef.vsCMTypeRefString:
                    return new StringType();

                case vsCMTypeRef.vsCMTypeRefBool:
                    return new BoolType();

                case vsCMTypeRef.vsCMTypeRefByte:
                case vsCMTypeRef.vsCMTypeRefDouble:
                case vsCMTypeRef.vsCMTypeRefInt:
                case vsCMTypeRef.vsCMTypeRefShort:
                case vsCMTypeRef.vsCMTypeRefFloat:
                case vsCMTypeRef.vsCMTypeRefLong:
                case vsCMTypeRef.vsCMTypeRefDecimal:
                    return new NumberType();

                default:
                    return TryResolveType(codeType);
            }
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

        private TypescriptType TryResolveType(CodeTypeRef codeType)
        {
            if (codeType.TypeKind == vsCMTypeRef.vsCMTypeRefArray)
            {
                return new ArrayType()
                {
                    ElementType = GetTypeScriptType(codeType.ElementType)
                };
            }
            

            return GetTypeScriptType(codeType.AsFullName);
        }

        public TypescriptType GetTypeScriptType(string typeFullName)
        {
            TypescriptType result;

            TypeScriptOutputType output = this.GetOutput(typeFullName);
            if (output != null)
            {
                result = new OutputType(output);
            }
            else
            { 
                if (IsGenericEnumerable(typeFullName))
                {
                    return new ArrayType
                    {
                        ElementType = GetTypeScriptType(UnwrapGenericType(typeFullName))
                    };
                }
                else if (IsNullable(typeFullName))
                {
                    return new NullableType
                    {
                        WrappedType = GetTypeScriptType(UnwrapGenericType(typeFullName))
                    };
                }

                var realType = TypeFullNameParser.Parse(typeFullName);

                if (realType.IsEnumerable()
                    || realType.IsArray())
                {
                    return new ArrayType()
                    {
                        ElementType = GetTypeScriptType(realType.TypeArgumentFullNames[0].FullName)
                    };
                }
                else if(realType.IsDictionary())
                {
                    return new DictionaryType()
                    {
                        KeyType = GetTypeScriptType(realType.TypeArgumentFullNames[0].FullName),
                        ElementType = GetTypeScriptType(realType.TypeArgumentFullNames[1].FullName)
                    };
                }

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
                    default:
                        result = new OutputType(
                            new TypeScriptLiteralType()
                            {
                                FullName = typeFullName
                            });
                        break;
                }
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
