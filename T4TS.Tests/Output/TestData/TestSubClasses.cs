using System.Collections.Generic;
using System.Linq;
using T4TS.Tests.Mocks;

namespace T4TS.Tests
{
    partial class MemberOutputAppenderTests
    {
        #region - Test Sub Classes -

        private List<TypeScriptModule> GetDataToRenderSubClasses(Settings settings)
        {
            var generator = new CodeTraverser(
                new MockSolution(
                    typeof(TestClass1)
                    ).Object, settings);
            return generator.GetAllInterfaces().ToList();
        }



        private const string ExpectedSubClassesResult = OutputHeader + @"
declare module External1 {
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestClass1+SubEnum **/
    export module TestClass1 {
        enum SubEnum {
            TheItem1 = 1,
            Item2 = 2,
            Item21 = 3,
            Item22 = 4,
            Item23 = 5,
            Item3 = 5,
            Item4 = 6,
        }
    }
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestClass1+SubClass **/
    export module TestClass1 {
        export interface TestSubClass {
            Id: number;
            Name: string;
        }
    }
    /** Generated from T4TS.Tests.MemberOutputAppenderTests+TestClass1 **/
    export interface TestClass1 {
        Id: number;
        Name: string;
        SubCls: External1.TestClass1.TestSubClass;
        SubEnm: External1.TestClass1.SubEnum;
    }
}
";

        [TypeScriptInterface(Module = "External1")]
        public class TestClass1
        {
            public enum SubEnum
            {
                TheItem1 = 1,
                Item2 = 2,
                Item21 = 3,
                Item22 = 4,
                Item23 = 5,
                Item3 = 5,
                Item4 = 6,
            }

            public int Id { get; set; }
            public string Name { get; set; }
            public SubClass SubCls { get; set; }
            public SubEnum SubEnm { get; set; }

            [TypeScriptInterface(Name = "TestSubClass")]
            public class SubClass
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

        #endregion
    }
}