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
            Settings = settings;
        }

        private static readonly string[] _genericCollectionTypeStarts =
        {
            "System.Collections.Generic.List<",
            "System.Collections.Generic.IList<",
            "System.Collections.Generic.ICollection<"
        };

        private const string NullableTypeStart = "System.Nullable<";

        /// <summary>
        /// Lookup table for "interface types", ie. non-builtin types (typically classes or unknown types). Keyed on the FullName of the type.
        /// </summary>
        private readonly Dictionary<string, InterfaceType> _interfaceTypes = new Dictionary<string, InterfaceType>();

        private readonly Dictionary<string, EnumType> _enumTypes = new Dictionary<string, EnumType>();

        public void AddInterfaceType(string typeFullName, InterfaceType interfaceType)
        {
            _interfaceTypes.Add(typeFullName, interfaceType);
        }
        public void AddEnumType(string typeFullName, EnumType enumType)
        {
            _enumTypes.Add(typeFullName, enumType);
        }

        public bool TryGetInterfaceType(string typeFullName, out InterfaceType interfaceType)
        {
            return _interfaceTypes.TryGetValue(typeFullName, out interfaceType);
        }

        public bool TryGetEnumType(string typeFullName, out EnumType enumType)
        {
            return _enumTypes.TryGetValue(typeFullName, out enumType);
        }

        public bool ContainsInterfaceType(string typeFullName)
        {
            return _interfaceTypes.ContainsKey(typeFullName);
        }
        public bool ContainsEnumType(string typeFullName)
        {
            return _enumTypes.ContainsKey(typeFullName);
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
                return new ArrayType
                {
                    ElementType = GetTypeScriptType(codeType.ElementType)
                };
            }

            return GetTypeScriptType(codeType.AsFullName);
        }

        public TypescriptType GetTypeScriptType(string typeFullName)
        {
            InterfaceType interfaceType;
            if (_interfaceTypes.TryGetValue(typeFullName, out interfaceType))
                return interfaceType;

            EnumType enumType;
            if (_enumTypes.TryGetValue(typeFullName, out enumType))
                return enumType;

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
                    if (Settings.UseNativeDates)
                        return new DateTimeType();
                    else
                        return new StringType();

                case "System.Boolean":
                    return new BoolType();

                default:
                    return new TypescriptType();
            }
        }

        private bool IsNullable(string typeFullName)
        {
            return typeFullName.StartsWith(NullableTypeStart);
        }

        public string UnwrapGenericType(string typeFullName)
        {
            int firstIndex = typeFullName.IndexOf('<');
            return typeFullName.Substring(firstIndex+1, typeFullName.Length - firstIndex- 2);
        }

        public static bool IsGenericEnumerable(string typeFullName)
        {
            return _genericCollectionTypeStarts.Any(typeFullName.StartsWith);
        }
    }
}
