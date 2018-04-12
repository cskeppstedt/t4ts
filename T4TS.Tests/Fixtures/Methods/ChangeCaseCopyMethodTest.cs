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
                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateConstructor(
                                builder.OutputSettings,
                                builder.TypeContext,
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateMethod(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
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
                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateConstructor(
                                builder.OutputSettings,
                                builder.TypeContext,
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateMethod(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
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
                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateConstructor(
                                builder.OutputSettings,
                                builder.TypeContext,
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateMethod(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toCamelCase: true));
                        });
                    })
                    .ToEqual(ExpectedOutputEnumerable);
        }

        [TestMethod]
        public void InheritanceModelHasExpectedOutputDirect()
        {
            // Expect
            new OutputForDirectBuilder(
                typeof(Inheritance.InheritanceModel),
                typeof(Inheritance.OtherInheritanceModel),
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
                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateConstructor(
                                builder.OutputSettings,
                                builder.TypeContext,
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.CreateMethod(
                                builder.OutputSettings,
                                builder.TypeContext,
                                name: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toCamelCase: true));
                        });
                    })
                    .ToEqual(InheritanceExpectedOutput);
        }

        private const string ExpectedOutputDirect = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel **/
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        constructor(other: T4TSTests.OtherType)
        {
            this.MyProperty = other.myProperty;
            this.SomeDateTime = other.someDateTime;
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
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
        constructor(other: T4TSTests.OtherType)
        {
            this.MyProperty = other.myProperty;
            this.SomeDateTime = other.someDateTime;
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
            result.myProperty = this.MyProperty;
            result.someDateTime = this.SomeDateTime;
            return result;
        }
    }
    /** Generated from T4TS.Tests.Fixtures.ExtendsExplicit.ExtendsExplicitModel **/
    export interface ExtendsExplicitModel {
        Basic: T4TS.BasicModel;
        constructor(other: T4TSTests.OtherType)
        {
            this.Basic = other.constructor();
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
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
        constructor(other: T4TSTests.OtherType)
        {
            this.MyProperty = other.myProperty;
            this.SomeDateTime = other.someDateTime;
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
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
        constructor(other: T4TSTests.OtherType)
        {
            this.NormalProperty = other.normalProperty;
            this.PrimitiveArray = new T4TS.TypeName[other.primitiveArray.length];
            for (var index3: number = 0; index3 < other.primitiveArray.length; index3++)
            {
                this.PrimitiveArray[index3] = other.primitiveArray;
            }
            this.PrimitiveList = other.primitiveList;
            this.InterfaceArray = new T4TS.TypeName[other.interfaceArray.length];
            for (var index3: number = 0; index3 < other.interfaceArray.length; index3++)
            {
                this.InterfaceArray[index3] = other.constructor();
            }
            this.InterfaceList = other.interfaceList;
            this.DeepArray = new T4TS.TypeName[other.deepArray.length];
            for (var index3: number = 0; index3 < other.deepArray.length; index3++)
            {
                this.DeepArray[index3] = new T4TS.TypeName[other.deepArray.length];
                for (var index4: number = 0; index4 < other.deepArray.length; index4++)
                {
                    this.DeepArray[index3][index4] = other.deepArray;
                }
            }
            this.DeepList = other.deepList;
            this.Generic = other.generic;
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
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

        public const string InheritanceExpectedOutput = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel **/
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        constructor(other: T4TSTests.OtherType)
        {
            this.MyProperty = other.myProperty;
            this.SomeDateTime = other.someDateTime;
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
            result.myProperty = this.MyProperty;
            result.someDateTime = this.SomeDateTime;
            return result;
        }
    }
}

declare module T4TS.Tests.Fixtures.Inheritance {
    /** Generated from T4TS.Tests.Fixtures.Inheritance.InheritanceModel **/
    export interface InheritanceModel extends T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel {
        OnInheritanceModel: T4TS.BasicModel;
        constructor(other: T4TSTests.OtherType)
            : base(other)
        {
            this.OnInheritanceModel = other.constructor();
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
            result.onInheritanceModel = this.ToOther();
            return result;
        }
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel **/
    export interface OtherInheritanceModel extends UNKNOWN_TYPE_T4TS.Example.Models.ModelFromDifferentProject {
        OnOtherInheritanceModel: T4TS.BasicModel;
        constructor(other: T4TSTests.OtherType)
            : base(other)
        {
            this.OnOtherInheritanceModel = other.constructor();
        }
        ToOther(): T4TSTests.OtherType
        {
            var result: T4TSTests.OtherType = new T4TSTests.OtherType();
            result.onOtherInheritanceModel = this.ToOther();
            return result;
        }
    }
}";
    }
}
