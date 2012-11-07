
/************************************************
  Generated file
************************************************/

module Api {

    /** Generated from T4TS.Example.Models.Barfoo **/
    export interface Barfoo {
        Number: number;
        Complex: Inherited;
        Name: string;
        DateTime: string;
    }

    /** Generated from T4TS.Example.Models.Foobar **/
    export interface Foobar {
        IntegerProperty: number;
        SomeString: string;
        NestedObject: Barfoo;
        NestedObjectArr: Barfoo[];
        NestedObjectList: Barfoo[];
    }

    /** Generated from T4TS.Example.Models.Inherited **/
    export interface Inherited {
        StringProperty: string;
        Integers: number[];
        Doubles: number[];
        [index: number]: Barfoo;
    }

}