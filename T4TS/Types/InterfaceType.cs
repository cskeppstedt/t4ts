using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class InterfaceType : TypescriptType
    {
        TypeScriptInterface tsInterface;

        public string QualifedModule 
        { 
            get 
            {
                return (this.tsInterface.Module != null)
                    ? this.tsInterface.Module.QualifiedName
                    : null;
            } 
        }

        public override string Name
        {
            get
            {
                return this.tsInterface.Name;
            }
        }

        public InterfaceType(TypeScriptInterface tsInterface)
        {
            this.tsInterface = tsInterface;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(QualifedModule))
                return base.ToString();

            return QualifedModule + "." + base.ToString();
        }
    }
}
