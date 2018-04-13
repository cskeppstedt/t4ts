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
                                baseName: "FromOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: true,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                baseName: "ToOther",
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
                                baseName: "FromOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: true,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                baseName: "ToOther",
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
                                baseName: "FromOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: true,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                baseName: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: false,
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
                typeof(Example.Models.ModelFromDifferentProject),
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
                                baseName: "FromOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: true,
                                toCamelCase: false));

                            outputType.Methods.Add(T4TS.Outputs.Custom.ChangeCaseCopyMethod.Create(
                                builder.OutputSettings,
                                builder.TypeContext,
                                baseName: "ToOther",
                                containingType: outputType,
                                otherTypeLiteral: otherTypeLiteral,
                                toContainingType: false,
                                toCamelCase: true));
                        });
                    })
                    .ToEqual(InheritanceExpectedOutput);
        }

        private const string ExpectedOutputDirect = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel */
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        FromOtherBasicModel(source: T4TSTests.OtherType): T4TS.BasicModel
        {
            this.MyProperty = source.myProperty;
            this.SomeDateTime = source.someDateTime;
            return this;
        }
        ToOtherBasicModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            target.myProperty = this.MyProperty;
            target.someDateTime = this.SomeDateTime;
            return target;
        }
    }
}";

        private const string ExpectedOutputNested = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel */
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        FromOtherBasicModel(source: T4TSTests.OtherType): T4TS.BasicModel
        {
            this.MyProperty = source.myProperty;
            this.SomeDateTime = source.someDateTime;
            return this;
        }
        ToOtherBasicModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            target.myProperty = this.MyProperty;
            target.someDateTime = this.SomeDateTime;
            return target;
        }
    }
    /** Generated from T4TS.Tests.Fixtures.ExtendsExplicit.ExtendsExplicitModel */
    export interface ExtendsExplicitModel {
        Basic: T4TS.BasicModel;
        FromOtherExtendsExplicitModel(source: T4TSTests.OtherType): T4TS.ExtendsExplicitModel
        {
            this.Basic = new T4TS.BasicModel().FromOtherBasicModel(source.basic);
            return this;
        }
        ToOtherExtendsExplicitModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            target.basic = this.Basic.ToOtherBasicModel(new T4TSTests.OtherType());
            return target;
        }
    }
}";

        private const string ExpectedOutputEnumerable = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel */
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        FromOtherBasicModel(source: T4TSTests.OtherType): T4TS.BasicModel
        {
            this.MyProperty = source.myProperty;
            this.SomeDateTime = source.someDateTime;
            return this;
        }
        ToOtherBasicModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            target.myProperty = this.MyProperty;
            target.someDateTime = this.SomeDateTime;
            return target;
        }
    }
    /** Generated from T4TS.Tests.Fixtures.Enumerable.EnumerableModel */
    export interface EnumerableModel {
        NormalProperty: number;
        PrimitiveArray: number[];
        PrimitiveList: number[];
        InterfaceArray: T4TS.BasicModel[];
        InterfaceList: T4TS.BasicModel[];
        DeepArray: number[][];
        DeepList: number[][];
        Generic: string[];
        FromOtherEnumerableModel(source: T4TSTests.OtherType): T4TS.EnumerableModel
        {
            this.NormalProperty = source.normalProperty;
            this.PrimitiveArray = new Array(source.primitiveArray.length);
            for (var index3: number = 0; index3 < source.primitiveArray.length; index3++)
            {
                this.PrimitiveArray[index3] = source.primitiveArray[index3];
            }
            this.PrimitiveList = new Array(source.primitiveList.length);
            for (var index3: number = 0; index3 < source.primitiveList.length; index3++)
            {
                this.PrimitiveList[index3] = source.primitiveList[index3];
            }
            this.InterfaceArray = new Array(source.interfaceArray.length);
            for (var index3: number = 0; index3 < source.interfaceArray.length; index3++)
            {
                this.InterfaceArray[index3] = new T4TS.BasicModel().FromOtherBasicModel(source.interfaceArray[index3]);
            }
            this.InterfaceList = new Array(source.interfaceList.length);
            for (var index3: number = 0; index3 < source.interfaceList.length; index3++)
            {
                this.InterfaceList[index3] = new T4TS.BasicModel().FromOtherBasicModel(source.interfaceList[index3]);
            }
            this.DeepArray = new Array(source.deepArray.length);
            for (var index3: number = 0; index3 < source.deepArray.length; index3++)
            {
                this.DeepArray[index3] = new Array(source.deepArray[index3].length);
                for (var index4: number = 0; index4 < source.deepArray[index3].length; index4++)
                {
                    this.DeepArray[index3][index4] = source.deepArray[index3][index4];
                }
            }
            this.DeepList = new Array(source.deepList.length);
            for (var index3: number = 0; index3 < source.deepList.length; index3++)
            {
                this.DeepList[index3] = new Array(source.deepList[index3].length);
                for (var index4: number = 0; index4 < source.deepList[index3].length; index4++)
                {
                    this.DeepList[index3][index4] = source.deepList[index3][index4];
                }
            }
            this.Generic = new Array(source.generic.length);
            for (var index3: number = 0; index3 < source.generic.length; index3++)
            {
                this.Generic[index3] = source.generic[index3];
            }
            return this;
        }
        ToOtherEnumerableModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            target.normalProperty = this.NormalProperty;
            target.primitiveArray = new Array(this.PrimitiveArray.length);
            for (var index3: number = 0; index3 < this.PrimitiveArray.length; index3++)
            {
                target.primitiveArray[index3] = this.PrimitiveArray[index3];
            }
            target.primitiveList = new Array(this.PrimitiveList.length);
            for (var index3: number = 0; index3 < this.PrimitiveList.length; index3++)
            {
                target.primitiveList[index3] = this.PrimitiveList[index3];
            }
            target.interfaceArray = new Array(this.InterfaceArray.length);
            for (var index3: number = 0; index3 < this.InterfaceArray.length; index3++)
            {
                target.interfaceArray[index3] = this.InterfaceArray[index3].ToOtherBasicModel(new T4TSTests.OtherType());
            }
            target.interfaceList = new Array(this.InterfaceList.length);
            for (var index3: number = 0; index3 < this.InterfaceList.length; index3++)
            {
                target.interfaceList[index3] = this.InterfaceList[index3].ToOtherBasicModel(new T4TSTests.OtherType());
            }
            target.deepArray = new Array(this.DeepArray.length);
            for (var index3: number = 0; index3 < this.DeepArray.length; index3++)
            {
                target.deepArray[index3] = new Array(this.DeepArray[index3].length);
                for (var index4: number = 0; index4 < this.DeepArray[index3].length; index4++)
                {
                    target.deepArray[index3][index4] = this.DeepArray[index3][index4];
                }
            }
            target.deepList = new Array(this.DeepList.length);
            for (var index3: number = 0; index3 < this.DeepList.length; index3++)
            {
                target.deepList[index3] = new Array(this.DeepList[index3].length);
                for (var index4: number = 0; index4 < this.DeepList[index3].length; index4++)
                {
                    target.deepList[index3][index4] = this.DeepList[index3][index4];
                }
            }
            target.generic = new Array(this.Generic.length);
            for (var index3: number = 0; index3 < this.Generic.length; index3++)
            {
                target.generic[index3] = this.Generic[index3];
            }
            return target;
        }
    }
}";

        public const string InheritanceExpectedOutput = @"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel */
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
        FromOtherBasicModel(source: T4TSTests.OtherType): T4TS.BasicModel
        {
            this.MyProperty = source.myProperty;
            this.SomeDateTime = source.someDateTime;
            return this;
        }
        ToOtherBasicModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            target.myProperty = this.MyProperty;
            target.someDateTime = this.SomeDateTime;
            return target;
        }
    }
}

declare module T4TS.Example.Models {
    /** Generated from T4TS.Example.Models.ModelFromDifferentProject */
    export interface ModelFromDifferentProject {
        Id: number;
        FromOtherModelFromDifferentProject(source: T4TSTests.OtherType): T4TS.Example.Models.ModelFromDifferentProject
        {
            this.Id = source.id;
            return this;
        }
        ToOtherModelFromDifferentProject(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            target.id = this.Id;
            return target;
        }
    }
}

declare module T4TS.Tests.Fixtures.Inheritance {
    /** Generated from T4TS.Tests.Fixtures.Inheritance.InheritanceModel */
    export interface InheritanceModel extends T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel {
        OnInheritanceModel: T4TS.BasicModel;
        FromOtherInheritanceModel(source: T4TSTests.OtherType): T4TS.Tests.Fixtures.Inheritance.InheritanceModel
        {
            super.FromOtherOtherInheritanceModel(source);
            this.OnInheritanceModel = new T4TS.BasicModel().FromOtherBasicModel(source.onInheritanceModel);
            return this;
        }
        ToOtherInheritanceModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            super.ToOtherOtherInheritanceModel(target);
            target.onInheritanceModel = this.OnInheritanceModel.ToOtherBasicModel(new T4TSTests.OtherType());
            return target;
        }
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel */
    export interface OtherInheritanceModel extends T4TS.Example.Models.ModelFromDifferentProject {
        OnOtherInheritanceModel: T4TS.BasicModel;
        FromOtherOtherInheritanceModel(source: T4TSTests.OtherType): T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel
        {
            super.FromOtherModelFromDifferentProject(source);
            this.OnOtherInheritanceModel = new T4TS.BasicModel().FromOtherBasicModel(source.onOtherInheritanceModel);
            return this;
        }
        ToOtherOtherInheritanceModel(target: T4TSTests.OtherType): T4TSTests.OtherType
        {
            super.ToOtherModelFromDifferentProject(target);
            target.onOtherInheritanceModel = this.OnOtherInheritanceModel.ToOtherBasicModel(new T4TSTests.OtherType());
            return target;
        }
    }
}";
    }
}
