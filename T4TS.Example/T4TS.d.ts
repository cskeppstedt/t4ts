
/************************************************
  Generated file
************************************************/

module Api {
    export interface Barfoo {
		No: number;
		Dbl: number;
		Name: string;
    }
    export interface Foobar {
		IntegerProperty: number;
		SomeString: string;
		NestedObject: Barfoo;
		NestedObjectArr: Barfoo[];
		NestedObjectList: Barfoo[];
    }
    export interface Inherited {
		StringProperty: string;
		[index: number]: Barfoo;
    }
}