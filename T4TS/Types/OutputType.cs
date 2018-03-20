using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class OutputType : TypescriptType
    {
        private TypeScriptOutputType output;

        public string QualifedModule 
        { 
            get 
            {
                return (this.output.Module != null)
                    ? this.output.Module.QualifiedName
                    : null;
            } 
        }

        public override string Name
        {
            get
            {
                return this.output.Name;
            }
        }

        public OutputType(TypeScriptOutputType output)
        {
            this.output = output;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(QualifedModule))
                return base.ToString();

            return QualifedModule + "." + base.ToString();
        }
    }
}
