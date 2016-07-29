using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Utils
{
    internal static class NamespaceUtil
    {
        internal static string GetNamespaceName(Type fromType)
        {
            string fullName = fromType.FullName;
            return fullName.Substring(0, fullName.LastIndexOf('.'));
        }

        internal static Dictionary<string, Type[]> GroupedByNamespace(IEnumerable<Type> types) 
        {
            return types
                .GroupBy(GetNamespaceName)
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}
