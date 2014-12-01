namespace T4TS
{
    public class NullableType : TypescriptType
    {
        public TypescriptType WrappedType { get; set; }

        public override string ToString()
        {
            return WrappedType.ToString();
        }
    }
}