using EnvDTE;
using System.Collections.Generic;
using System.Linq;

namespace T4TS
{
    public class TypeContext
    {
        public Settings Settings { get; private set; }
        public TypeContext(Settings settings)
        {
            this.Settings = settings;
        }

        private static readonly string[] genericCollectionTypeStarts = new string[] {
            "System.Collections.Generic.List<",
            "System.Collections.Generic.IList<",
            "System.Collections.Generic.ICollection<"
        };

        private static readonly string nullableTypeStart = "System.Nullable<";

        /// <summary>
        /// Lookup table for "interface types", ie. non-builtin types (typically classes or unknown types). Keyed on the FullName of the type.
        /// </summary>
        private Dictionary<string, InterfaceType> interfaceTypes = new Dictionary<string, InterfaceType>();

        public void AddInterfaceType(string typeFullName, InterfaceType interfaceType)
        {
            interfaceTypes.Add(typeFullName, interfaceType);
        }

        public bool TryGetInterfaceType(string typeFullName, out InterfaceType interfaceType)
        {
            return interfaceTypes.TryGetValue(typeFullName, out interfaceType);
        }

        public bool ContainsInterfaceType(string typeFullName)
        {
            return interfaceTypes.ContainsKey(typeFullName);
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
            InterfaceType interfaceType;
            if (interfaceTypes.TryGetValue(typeFullName, out interfaceType))
                return interfaceType;

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

            switch (typeFullName)
            {
                case "System.Guid":
                    return new GuidType();

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
                    return new NumberType();

                case "System.String":
                    return new StringType();

                case "System.DateTime":
                case "System.DateTimeOffset":
                    if (Settings.UseNativeDates)
                        return new DateTimeType();
                    else
                        return new StringType();

                default:
                    return new TypescriptType();
            }
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
