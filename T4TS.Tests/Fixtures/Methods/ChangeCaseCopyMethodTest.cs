using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS.Tests.Fixtures.Basic;
using T4TS.Tests.Utils;

namespace T4TS.Tests.Fixtures.Methods
{
    [TestClass]
    public class ChangeCaseCopyMethodTest
    {
        [TestMethod]
        public void BasicModelHasExpectedOutputDirect()
        {
            // Expect
            new OutputForDirectBuilder(
                typeof(BasicModel))
                    .Edit((builder) =>
                    {
                        builder.DirectSettings.NamespaceToModuleMap.Add(
                            typeof(BasicModel).Namespace,
                            "T4TS");
                        builder.TraverserSettings.TypeDecorators.Add((outputType) =>
                        {
                            string otherTypeLiteral = "T4TSTests.OtherType";
                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "FromOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: true,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: false,
                                toCamelCase: true));
                        });
                    })
                    .ToEqual(ExpectedOutputDirect);
        }

        [TestMethod]
        public void NestedModelHasExpectedOutputDirect()
        {
            // Expect
            new OutputForDirectBuilder(
                typeof(ExtendsExplicit.ExtendsExplicitModel),
                typeof(BasicModel))
                    .Edit((builder) =>
                    {
                        builder.DirectSettings.NamespaceToModuleMap.Add(
                            typeof(BasicModel).Namespace,
                            "T4TS");
                        builder.DirectSettings.NamespaceToModuleMap.Add(
                            typeof(ExtendsExplicit.ExtendsExplicitModel).Namespace,
                            "T4TS");

                        builder.TraverserSettings.TypeDecorators.Add((outputType) =>
                        {
                            string otherTypeLiteral = "T4TSTests.OtherType";
                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "FromOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: true,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: false,
                                toCamelCase: true));
                        });
                    })
                    .ToEqual(ExpectedOutputNested);
        }

        [TestMethod]
        public void EnumerableModelHasExpectedOutputDirect()
        {
            // Expect
            new OutputForDirectBuilder(
                typeof(Enumerable.EnumerableModel),
                typeof(BasicModel))
                    .Edit((builder) =>
                    {
                        builder.DirectSettings.NamespaceToModuleMap.Add(
                            typeof(BasicModel).Namespace,
                            "T4TS");
                        builder.DirectSettings.NamespaceToModuleMap.Add(
                            typeof(Enumerable.EnumerableModel).Namespace,
                            "T4TS");

                        builder.TraverserSettings.TypeDecorators.Add((outputType) =>
                        {
                            string otherTypeLiteral = "T4TSTests.OtherType";
                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "FromOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: true,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: false,
                                toCamelCase: true));
                        });
                    })
                    .ToEqual(ExpectedOutputEnumerable);
        }

        private const string ExpectedOutputDirect = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel **/
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        static FromOther( other: T4TS.BasicModel): T4TS.BasicModel
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.MyProperty = other.myProperty;
            result.SomeDateTime = other.someDateTime;
            return result;
        }
        ToOther(): T4TSTests.OtherType
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.myProperty = this.MyProperty;
            result.someDateTime = this.SomeDateTime;
            return result;
        }
    }
}";
        private const string ExpectedOutputNested = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel **/
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        static FromOther( other: T4TS.BasicModel): T4TS.BasicModel
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.MyProperty = other.myProperty;
            result.SomeDateTime = other.someDateTime;
            return result;
        }
        ToOther(): T4TSTests.OtherType
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.myProperty = this.MyProperty;
            result.someDateTime = this.SomeDateTime;
            return result;
        }
    }
    /** Generated from T4TS.Tests.Fixtures.ExtendsExplicit.ExtendsExplicitModel **/
    export interface ExtendsExplicitModel {
        Basic: T4TS.BasicModel;
        static FromOther( other: T4TS.ExtendsExplicitModel): T4TS.ExtendsExplicitModel
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.Basic = T4TS.BasicModel.FromOther(other.basic);
            return result;
        }
        ToOther(): T4TSTests.OtherType
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.basic = this.ToOther();
            return result;
        }
    }
}";

        private const string ExpectedOutputEnumerable = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel **/
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        static FromOther( other: T4TS.BasicModel): T4TS.BasicModel
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.MyProperty = other.myProperty;
            result.SomeDateTime = other.someDateTime;
            return result;
        }
        ToOther(): T4TSTests.OtherType
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.myProperty = this.MyProperty;
            result.someDateTime = this.SomeDateTime;
            return result;
        }
    }
    /** Generated from T4TS.Tests.Fixtures.Enumerable.EnumerableModel **/
    export interface EnumerableModel {
        NormalProperty: number;
        PrimitiveArray: number[];
        PrimitiveList: number[];
        InterfaceArray: T4TS.BasicModel[];
        InterfaceList: T4TS.BasicModel[];
        DeepArray: number[][];
        DeepList: number[][];
        Generic: string[];
        static FromOther( other: T4TS.EnumerableModel): T4TS.EnumerableModel
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.NormalProperty = other.normalProperty;
            result.PrimitiveArray = new T4TS.TypeName[other.primitiveArray.length];
            for (var index3: number = 0; index3 < other.primitiveArray.length; index3++)
            {
                result.PrimitiveArray[index3] = other.primitiveArray;
            }
            result.PrimitiveList = other.primitiveList;
            result.InterfaceArray = new T4TS.TypeName[other.interfaceArray.length];
            for (var index3: number = 0; index3 < other.interfaceArray.length; index3++)
            {
                result.InterfaceArray[index3] = T4TS.BasicModel.FromOther(other.interfaceArray);
            }
            result.InterfaceList = other.interfaceList;
            result.DeepArray = new T4TS.TypeName[other.deepArray.length];
            for (var index3: number = 0; index3 < other.deepArray.length; index3++)
            {
                result.DeepArray[index3] = new T4TS.TypeName[other.deepArray.length];
                for (var index4: number = 0; index4 < other.deepArray.length; index4++)
                {
                    result.DeepArray[index3][index4] = other.deepArray;
                }
            }
            result.DeepList = other.deepList;
            result.Generic = other.generic;
            return result;
        }
        ToOther(): T4TSTests.OtherType
        {
            T4TSTests.OtherType result = new T4TSTests.OtherType();
            result.normalProperty = this.NormalProperty;
            result.primitiveArray = new T4TS.TypeName[this.PrimitiveArray.length];
            for (var index3: number = 0; index3 < this.PrimitiveArray.length; index3++)
            {
                result.primitiveArray[index3] = this.PrimitiveArray;
            }
            result.primitiveList = this.PrimitiveList;
            result.interfaceArray = new T4TS.TypeName[this.InterfaceArray.length];
            for (var index3: number = 0; index3 < this.InterfaceArray.length; index3++)
            {
                result.interfaceArray[index3] = this.ToOther();
            }
            result.interfaceList = this.InterfaceList;
            result.deepArray = new T4TS.TypeName[this.DeepArray.length];
            for (var index3: number = 0; index3 < this.DeepArray.length; index3++)
            {
                result.deepArray[index3] = new T4TS.TypeName[this.DeepArray.length];
                for (var index4: number = 0; index4 < this.DeepArray.length; index4++)
                {
                    result.deepArray[index3][index4] = this.DeepArray;
                }
            }
            result.deepList = this.DeepList;
            result.generic = this.Generic;
            return result;
        }
    }
}";
    }
}
