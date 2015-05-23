﻿/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

// -- Begin global interfaces
    /** Generated from T4TS.Example.Models.Barfoo **/
    interface Barfoo {
        number: number;
        complex: T4TS.OverridenName;
        name: string;
        dateTime: Date;
        aValue: { [name: string]: any};
    }
// -- End global interfaces

declare module External {
    /** Generated from T4TS.Example.Models.ModelFromDifferentProject **/
    export interface ModelFromDifferentProject {
        id: number;
    }
}

declare module Fooz {
    /** Generated from T4TS.Example.Models.Foobar **/
    export interface IFoobar {
        OverrideAll?: any;
        aGuid: string;
        recursive: Fooz.IFoobar;
        nullableInt: number;
        nullableDouble: number;
        nestedObjectArr: Barfoo[];
        nestedObjectList: Barfoo[];
        twoDimensions: string[][];
        threeDimensions: Barfoo[][][];
        camelCasePlease: number;
        doNotIgnoreMe: number;
    }
}

declare module Override {
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultModuleOverrideModel **/
    export interface DefaultModuleOverrideModel {
        someProp: string;
    }
}

declare module SomeModule {
    /** Generated from T4TS.Tests.Fixtures.Options.Override.ModuleNameOverrideModel **/
    export interface ModuleNameOverrideModel {
        someThirdProp: string;
    }
}

declare module T4TS {
    /** Generated from T4TS.Example.Models.SomeEntity **/
    export interface someEntity extends externalJSModule.Entity {
        number: number;
        name: string;
        dateTime: Date;
        aValue: { [name: string]: any};
    }
    /** Generated from T4TS.Example.Models.InheritanceTest1 **/
    export interface InheritanceTest1 extends Barfoo {
        someString: string;
        recursive: Fooz.IFoobar;
    }
    /** Generated from T4TS.Example.Models.InheritanceTest2 **/
    export interface InheritanceTest2 extends T4TS.InheritanceTest1 {
        someString2: string;
        recursive2: Fooz.IFoobar;
    }
    /** Generated from T4TS.Example.Models.InheritanceTest3 **/
    export interface InheritanceTest3 extends T4TS.OverridenName {
        someString3: string;
        recursive3: Fooz.IFoobar;
    }
    /** Generated from T4TS.Example.Models.InheritanceTest4 **/
    export interface InheritanceTest4 {
        someString4: string;
        recursive4: Fooz.IFoobar;
    }
    /** Generated from T4TS.Example.Models.InheritanceTestExternal **/
    export interface InheritanceTestExternal extends External.ModelFromDifferentProject {
    }
    /** Generated from T4TS.Example.Models.Inherited **/
    export interface OverridenName {
        OtherName?: string;
        integers: number[];
        doubles: number[];
        twoDimList: number[][];
        [index: number]: Barfoo;
    }
    /** Generated from T4TS.Example.Models.Partial **/
    export interface Partial {
        fromFirstClass: string;
    }
    /** Generated from T4TS.Example.Models.Partial **/
    export interface Partial {
        fromSecondClass: string;
        alsoSecondClass: boolean;
    }
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel **/
    export interface BasicModel {
        myProperty: number;
        someDateTime: Date;
    }
    /** Generated from T4TS.Tests.Fixtures.Dictionary.DictionaryModel **/
    export interface DictionaryModel {
        intKey: { [name: number]: T4TS.BasicModel};
        stringKey: { [name: string]: T4TS.BasicModel};
        [index: number]: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Fixtures.Enumerable.EnumerableModel **/
    export interface EnumerableModel {
        normalProperty: number;
        primitiveArray: number[];
        primitiveList: number[];
        interfaceArray: T4TS.BasicModel[];
        interfaceList: T4TS.BasicModel[];
        deepArray: number[][];
        deepList: number[][];
        generic: string[];
    }
    /** Generated from T4TS.Tests.Fixtures.ExternalProp.ExternalPropModel **/
    export interface ExternalPropModel {
        basic: T4TS.BasicModel;
        external: External.ModelFromDifferentProject;
    }
    /** Generated from T4TS.Tests.Fixtures.Indexed.IndexedComplexModel **/
    export interface IndexedComplexModel {
        someProp: number;
        [index: number]: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Fixtures.Indexed.IndexedPrimitiveModel **/
    export interface IndexedPrimitiveModel {
        someProp: number;
        [index: number]: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.InheritanceModel **/
    export interface InheritanceModel extends T4TS.OtherInheritanceModel {
        onInheritanceModel: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel **/
    export interface OtherInheritanceModel extends External.ModelFromDifferentProject {
        onOtherInheritanceModel: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Fixtures.Nullable.NullableModel **/
    export interface NullableModel {
        nullableInt: number;
        nullableDouble: number;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultCamelCaseMemberNamesModel **/
    export interface DefaultCamelCaseMemberNamesModel {
        someProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultCamelCaseMemberNamesOverrideModel **/
    export interface DefaultCamelCaseMemberNamesOverrideModel {
        SomeProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultInterfaceNamePrefixModel **/
    export interface DefaultInterfaceNamePrefixModel {
        someProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultInterfaceNamePrefixOverrideModel **/
    export interface PrefixOverrideDefaultInterfaceNamePrefixOverrideModel {
        OverrideName: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultModuleModel **/
    export interface DefaultModuleModel {
        someProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultOptionalModel **/
    export interface DefaultOptionalModel {
        someProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.DefaultOptionalOverrideModel **/
    export interface DefaultOptionalOverrideModel {
        someProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Default.UseNativeDatesModel **/
    export interface UseNativeDatesModel {
        someDateTime: Date;
        someDateTimeOffset: Date;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Override.InterfaceNamePrefixModel **/
    export interface FooInterfaceNamePrefixModel {
        someProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Override.InterfaceNameOverrideModel **/
    export interface Bar {
        someOtherProp: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Override.MemberNameOverrideModel **/
    export interface MemberNameOverrideModel {
        OverriddenName: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Override.MemberOptionalModel **/
    export interface MemberOptionalModel {
        member?: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Override.MemberCamelCaseModel **/
    export interface MemberCamelCaseModel {
        memberName: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Override.MemberTypeModel **/
    export interface MemberTypeModel {
        notANumber: number;
    }
    /** Generated from T4TS.Tests.Fixtures.Options.Override.MemberIgnoreModel **/
    export interface MemberIgnoreModel {
        notIgnored: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Partial.PartialModel **/
    export interface PartialModel extends External.ModelFromDifferentProject {
        onPartialModel: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Fixtures.Partial.PartialModel **/
    export interface PartialModel extends External.ModelFromDifferentProject {
        onOtherPartialModel: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Traversal.Models.LocalModel **/
    export interface LocalModel {
        id: number;
        optional?: string;
    }
    /** Generated from T4TS.Tests.Traversal.Models.ReservedPropModel **/
    export interface ReservedPropModel {
        class: string;
        readonly: string;
        public?: boolean;
    }
}
