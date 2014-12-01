namespace T4TS
{
    public class EnumType : TypescriptType
    {
        public EnumType(TypeScriptEnumAttributeValues values)
        {
            AttributeValues = values;
        }

        public EnumType(string name)
        {
            AttributeValues = new TypeScriptEnumAttributeValues
            {
                Name = name
            };
        }

        public TypeScriptEnumAttributeValues AttributeValues { get; private set; }

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