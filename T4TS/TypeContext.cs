using EnvDTE;
using System.Collections.Generic;
using System.Linq;

namespace T4TS
{
    public class TypeContext
    {
        private bool useNativeDates;

        private IDictionary<string, TypeScriptModule> modulesByName =
            new Dictionary<string, TypeScriptModule>();
        private IDictionary<string, TypeScriptInterface> interfacesByFullName =
            new Dictionary<string, TypeScriptInterface>();


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
            TypeScriptInterface result = this.GetOrCreateInterface(
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

        public TypeScriptInterface GetOrCreateInterface(
            string fullName,
            out bool created)
        {
            TypeScriptInterface result;
            if (this.interfacesByFullName.TryGetValue(
                fullName,
                out result))
            {
                created = false;
            }
            else
            {
                result = new TypeScriptInterface()
                {
                    FullName = fullName
                };
                this.interfacesByFullName.Add(
                    fullName,
                    result);
                created = true;
            }
            return result;
        }

        public IEnumerable<TypeScriptModule> GetModules()
        {
            return this.modulesByName.Values
                .OrderBy((module) => module.QualifiedName);
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

            TypeScriptInterface interfaceType;
            if (this.interfacesByFullName.TryGetValue(
                typeFullName,
                out interfaceType))
            {
                result = new InterfaceType(interfaceType);
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

                if (realType.IsEnumerable())
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
                        bool interfaceCreated;
                        interfaceType = this.GetOrCreateInterface(
                            typeFullName,
                            out interfaceCreated);
                        result = new InterfaceType(interfaceType);
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
