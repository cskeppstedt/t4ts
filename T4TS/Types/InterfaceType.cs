using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class InterfaceType : TypescriptType
    {
        public TypeScriptInterfaceAttributeValues AttributeValues { get; private set; }

        public string QualifedModule 
        { 
            get 
            {
                if (AttributeValues == null)
                    return null;

                return AttributeValues.Module; 
            } 
        }

        public override string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(AttributeValues.NamePrefix))
                    return AttributeValues.NamePrefix + AttributeValues.Name;

                return AttributeValues.Name;
            }
        }

        public InterfaceType(TypeScriptInterfaceAttributeValues values)
        {
            AttributeValues = values;
        }

        public InterfaceType(string name)
        {
            AttributeValues = new TypeScriptInterfaceAttributeValues
            {
                Name = name
            };
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(QualifedModule))
                return base.ToString();

            return QualifedModule + "." + base.ToString();
        }
    }
}
