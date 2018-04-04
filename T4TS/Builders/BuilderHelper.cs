using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Builders
{
    public class BuilderHelper
    {
        public static bool IsValidBaseType(TypeName typeName)
        {
            Assembly systemAssembly = typeof(Object).Assembly;
            Type systemType = systemAssembly.GetType(typeName.UniversalName);
            return (systemType == null
                || (systemType != typeof(object)
                    && systemType.Namespace != typeof(IList<>).Namespace
                    && systemType.Namespace != typeof(System.Collections.IList).Namespace));
        }
    }
}
