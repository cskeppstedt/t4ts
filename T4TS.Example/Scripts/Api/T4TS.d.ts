
/************************************************
  Generated file
************************************************/
module T4TS {

    /** Generated from T4TS.Example.Models.Barfoo **/
    export interface Barfoo {
        Number: number;
        Complex: T4TS.Inherited;
        Name: string;
        DateTime: string;
    }

    /** Generated from T4TS.Example.Models.Foobar **/
    export interface Foobar {
        IntegerProperty: number;
        SomeString: string;
        NestedObject: T4TS.Barfoo;
        NestedObjectArr: T4TS.Barfoo[];
        NestedObjectList: T4TS.Barfoo[];
        TwoDimensions: string[][];
        ThreeDimensions: T4TS.Barfoo[][][];
    }

    /** Generated from T4TS.Example.Models.Inherited **/
    export interface Inherited {
        StringProperty: string;
        Integers: number[];
        Doubles: number[];
        TwoDimList: number[][];
        [index: number]: T4TS.Barfoo;
    }

}