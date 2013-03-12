namespace T4TS.Example.Models
{
    [TypeScriptInterface]
    public class InheritanceTest1 : Barfoo
    {
        public string SomeString { get; set; }
        public Foobar Recursive { get; set; }
    }
}