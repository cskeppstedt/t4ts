using System;
using System.Collections.Generic;

namespace T4TS.Example.Models
{
    [TypeScriptInterface(Name = "someEntity", ExistentExternalTypeName = "externalJSModule.Entity")]
    public class SomeEntity
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public Dictionary<string, object> aValue { get; set; }
    }
}
