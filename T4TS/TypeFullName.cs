using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{

    public class TypeFullName
    {
        public string FullName { get; private set; }
        public TypeFullName[] TypeArgumentFullNames { get; private set; }

        public TypeFullName(string name, params TypeFullName[] arguments)
        {
            this.FullName = name;
            this.TypeArgumentFullNames = arguments;
        }

        public static TypeFullName FromString(string name)
        {
            return TypeFullNameParser.Parse(name);
        }

        public override bool Equals(object obj)
        {
            if (obj == this) return true;
            if (obj == null) return false;
            if (obj is TypeFullName)
            {
                return obj.ToString() == this.ToString();
            }
            return base.Equals(obj);
        }
        public override string ToString()
        {
            if (TypeArgumentFullNames == null || TypeArgumentFullNames.Length == 0)
                return FullName;
            string nameWithGenerics = FullName + "<";
            foreach (var arg in TypeArgumentFullNames)
            {
                nameWithGenerics += arg.ToString() + ",";

            }
            if (nameWithGenerics.EndsWith(","))
            {
                nameWithGenerics = nameWithGenerics.Substring(0, nameWithGenerics.Length - 1);
            }
            return nameWithGenerics + ">";
        }
    }

}
