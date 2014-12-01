namespace T4TS
{
    public class ArrayType : TypescriptType
    {
        public TypescriptType ElementType { get; set; }

        public override string ToString()
        {
            return ElementType + "[]";
        }
    }
}