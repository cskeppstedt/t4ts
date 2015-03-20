using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using T4TS.Tests.Mocks;

namespace T4TS.Tests
{
    partial class MemberOutputAppenderTests
    {
        #region - Test Data Contract -

        private List<TypeScriptModule> GetDataToRenderDataContract(Settings settings)
        {
            var generator = new CodeTraverser(
                new MockSolution(
                    typeof(TestDataClass)
                    ).Object, settings);
            return generator.GetAllInterfaces().ToList();
        }

        private class JsonIgnoreAttribute : Attribute {}

        private const string ExpectedDataContractResult = OutputHeader + @"
declare module T4TS {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestDataClass **/
    export interface TestDataClass {
        Id: number;
        Name: string;
    }
}
";
        [DataContract]
        public class TestDataClass
        {
            public int Id { get; set; }
            public string Name { get; set; }

            [JsonIgnore]
            public string JsonIgnored { get; set; }
        }

        #endregion
    }
}