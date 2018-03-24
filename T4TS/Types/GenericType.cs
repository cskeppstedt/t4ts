using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class GenericType : TypescriptType
    {
        public TypescriptType BaseType { get; set; }
        public IList<TypescriptType> TypeArguments { get; set; }

        public override string ToString()
        {
            TypeFullName fullName = new TypeFullName(
                this.BaseType.Name,
                this.TypeArguments
                    .Select(
                        (typeArgument) => new TypeFullName(typeArgument.Name))
                    .ToArray());
            return fullName.ToString();
        }
    }
}
