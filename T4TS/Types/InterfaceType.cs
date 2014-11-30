namespace T4TS
{
    public class InterfaceType : TypescriptType
    {
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

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(QualifedModule))
                return base.ToString();

            return QualifedModule + "." + base.ToString();
        }
    }
}