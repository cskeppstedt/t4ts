/// <reference path="../../T4TS.d.ts" />

module App {
    export class Test {
        private partialClass: T4TS.Partial;

        constructor() {

            this.partialClass = {
                FromFirstClass: '',
                FromSecondClass: '',
                AlsoSecondClass: true
            };

            // Make an AJAX post and get some data from the server.
            // In the callback, you can specify that the data is of a certain type:
            $.post('./example', {}, (data: Fooz.IFoobar) => {

                // Intellisense support for the properties:
                alert(data.NestedObjectArr[0].Name);
                alert(data.Recursive.OverrideAll ? "1" : "0");

                // When using lib functions (such as $.each) you need to help
                // by explicitly typing the object in the callback:
                $.each(data.NestedObjectArr, (i, v: T4TS.InheritanceTest1) => {
                    alert(v.SomeString);
                });
            });
        }

    }
}