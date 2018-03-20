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

        public bool IsEnumerable()
        {
            return this.FullName == "System.Collections.Generic.IEnumerable";
        }

        public bool IsArray()
        {
            return this.FullName.EndsWith("[]");
        }

        public bool IsDictionary()
        {
            switch(this.FullName)
            {
                case "System.Collections.Generic.Dictionary":
                case "System.Collections.Generic.IDictionary":
                    return true;

                default:
                    return false;
            }
        }

        public static TypeFullName FromString(string name)
        {
            return TypeFullNameParser.Parse(name);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            if (obj == null)
                return false;

            if (obj is TypeFullName)
                return obj.ToString() == this.ToString();
            
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hashCode = 17;

            if (this.TypeArgumentFullNames != null)
                hashCode = (hashCode * 23) + this.TypeArgumentFullNames.GetHashCode();

            if (this.FullName != null)
                hashCode = (hashCode * 29) + this.FullName.GetHashCode();

            return hashCode;
        }

        public override string ToString()
        {
            if (TypeArgumentFullNames == null || TypeArgumentFullNames.Length == 0)
                return FullName;

            string nameWithGenerics = FullName + "<";
            foreach (var arg in TypeArgumentFullNames)
                nameWithGenerics += arg.ToString() + ",";
            
            if (nameWithGenerics.EndsWith(","))
                nameWithGenerics = nameWithGenerics.Substring(0, nameWithGenerics.Length - 1);
            
            return nameWithGenerics + ">";
        }
    }

}
