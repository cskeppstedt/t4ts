using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using T4TS.Example.Models;
using T4TS.Tests.Mocks;

namespace T4TS.Tests
{
    partial class MemberOutputAppenderTests
    {
        #region - Test Inherited Generic Data -

        private List<TypeScriptModule> GetInheritedGenericDataToRender(Settings settings)
        {
            var generator = new CodeTraverser(
                new MockSolution(
/*
                    typeof(SimpleParentDataTest),
                    typeof(SimpleDataTest)
*/
                    typeof(EntityObject<Device>),
                    typeof(EntityObjectId<Device>),
                    typeof(Device)
                    ).Object, settings);
            return generator.GetAllInterfaces().ToList();
        }

        private const string ExpectedInheritedGenericResult = OutputHeader + @"
declare module T4TS {
    /** Generated from T4TS.Example.Models.EntityObject`1[[T4TS.Example.Models.Device, T4TS.Example.Models, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]] **/
    export interface EntityObject {
    }
    /** Generated from T4TS.Example.Models.EntityObjectId`1[[T4TS.Example.Models.Device, T4TS.Example.Models, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]] **/
    export interface EntityObjectId extends T4TS.EntityObject {
        ApiKey: string;
        UserId: string;
        Id: string;
    }
    /** Generated from T4TS.Example.Models.Device **/
    export interface Device extends T4TS.EntityObjectId {
        Name: string;
    }
}
";

        #endregion
    }

}