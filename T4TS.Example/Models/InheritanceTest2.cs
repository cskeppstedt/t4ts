namespace T4TS.Example
{
    [TypeScriptInterface]
    public class InheritanceTest2 : InheritanceTest1
    {
        public string SomeString2 { get; set; }
        public Foobar Recursive2 { get; set; }
    }
}