using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace T4TS
{
    public class TypeScriptInterfaceMember
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        private string _docComment;
        public string DocComment {
          get { return _docComment; }
          set {
            _docComment = value;
            if(value == null) { return; }
            //strip the '<doc> </doc>' xml tags
            if(value.StartsWith("<doc>") && value.EndsWith("</doc>")) {
              _docComment = value.Substring(5, value.Length - 11).Trim();
            } else { _docComment = value; }
          }
        }
        public TypescriptType Type { get; set; }
        public bool Optional { get; set; }
        //public string FullName { get; set; }
        public bool Ignore { get; set; }
    }
}
