namespace T4TS
{
    public class TypescriptType
    {
        public virtual string Name
        {
            get { return "any"; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}