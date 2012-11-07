/// <reference path="../Api/T4TS.d.ts" />
/// <reference path="../lib/jquery-1.8.d.ts" />

module App {
    export class Test {
        
        constructor () {
            // Make an AJAX post and get some data from the server.
            // In the callback, you can specify that the data is of a certain type:
            $.post('./example', {}, (data: Api.Foobar) => {
                
                // Intellisense support for the properties:
                alert(data.NestedObject.Name);
                alert(data.NestedObject.Complex[1].Number.toString());

                // When using lib functions (such as $.each) you need to help
                // by explicitly typing the object in the callback:
                $.each(data.NestedObjectArr, (i, v: Api.Barfoo) => {
                    alert(v.DateTime);
                });
            });
        }

    }
}