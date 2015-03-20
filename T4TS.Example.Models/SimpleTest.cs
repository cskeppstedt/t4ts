using System;
using System.Runtime.Serialization;

namespace T4TS.Example.Models
{
    [DataContract]
    public class SimpleParentDataTest
    {
        [DataMember] public static int ParentPublicStatic { get; set; }

        [DataMember]
        public int ParentPublic { get; set; }
    }

    [DataContract]
    public class SimpleDataTest : SimpleParentDataTest
    {
        [DataMember] public int _public;
        [DataMember] protected int _protected;
        [DataMember] internal int _internal;
        [DataMember] private int _private;
        [DataMember] public int Public { get; set; }
        [DataMember] protected int Protected { get; set; }
        [TypeScriptMember] [DataMember] internal int Internal { get; set; }
        [DataMember] private int Private { get; set; }

        [DataMember] public static int PublicStatic { get; set; }

    }

    [TypeScriptInterface]
    public class SimpleTest : SimpleParentDataTest
    {
        [TypeScriptMember] public int _public;
        [TypeScriptMember] protected int _protected;
        [TypeScriptMember] internal int _internal;
        [TypeScriptMember] private int _private;
        public int Public { get; set; }
        [TypeScriptMember] protected int Protected { get; set; }
        [TypeScriptMember] internal int Internal { get; set; }
        private int Private { get; set; }

        public static int PublicStatic { get; set; }
    }

}
