using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class CustomType: TypescriptType
    {
        private string m_name;

        public override string Name
        {
            get { return m_name; }
        }
        
        public string QualifedModule { get; private set; }

        public CustomType(string name, string qualifiedModule=null)
        {
            m_name = name;
            this.QualifedModule  = qualifiedModule;
        }

        public override string ToString()
        {
            return QualifedModule + "." + base.ToString();
        }
    }
}
