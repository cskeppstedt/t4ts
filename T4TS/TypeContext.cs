using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeContext
    {
        private static readonly string[] genericCollectionTypeStarts = new string[] {
            "System.Collections.Generic.List<",
            "System.Collections.Generic.IList<",
            "System.Collections.Generic.ICollection<"
        };

        /// <summary>
        /// Lookup table for "custom types", ie. non-builtin types. Keyed on the FullName of the type.
        /// </summary>
        private Dictionary<string, CustomType> customTypes = new Dictionary<string, CustomType>();

        public void AddCustomType(string typeFullName, CustomType customType)
        {
            customTypes.Add(typeFullName, customType);
        }

        public bool TryGetCustomType(string typeFullName, out CustomType customType)
        {
            return customTypes.TryGetValue(typeFullName, out customType);
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

            if (genericCollectionTypeStarts.Any(s => codeType.AsFullName.StartsWith(s)))
            {
                string fullName = UnwrapGenericType(codeType);
                return TryResolveEnumerableType(fullName);
            }

            return GetTypeScriptType(codeType.AsFullName);
        }

        private ArrayType TryResolveEnumerableType(string typeFullName)
        {
            return new ArrayType
            {
                ElementType = GetTypeScriptType(typeFullName)
            };
        }

        public TypescriptType GetTypeScriptType(string typeFullName)
        {
            CustomType customType;
            if (customTypes.TryGetValue(typeFullName, out customType))
                return customType;

            switch (typeFullName)
            {
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
                case "System.DateTime":
                    return new StringType();

                default:
                    return new TypescriptType();
            }
        }

        public string UnwrapGenericType(CodeTypeRef codeType)
        {
            return UnwrapGenericType(codeType.AsFullName);
        }

        public string UnwrapGenericType(string typeFullName)
        {
            return typeFullName.Split('<', '>')[1];
        }

        public bool IsGenericEnumerable(string typeFullName)
        {
            return genericCollectionTypeStarts.Any(t => typeFullName.StartsWith(t));
        }
    }
}
