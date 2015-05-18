using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class DictionaryType : TypescriptType
    {
        public TypescriptType KeyType { get; set; }
        public TypescriptType ElementType { get; set; }

        public override string ToString()
        {
            return "{ [name: NameType]: ValueType}".Replace("NameType", KeyType.ToString()).Replace("ValueType", ElementType.ToString());
        }
    }
}
