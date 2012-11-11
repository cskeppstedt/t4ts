var App;
(function (App) {
    var Test = (function () {
        function Test() {
            $.post('./example', {
            }, function (data) {
                alert(data.NestedObject.Name);
                alert(data.NestedObject.Complex[1].Number.toString());
                $.each(data.NestedObjectArr, function (i, v) {
                    alert(v.DateTime);
                });
            });
        }
        return Test;
    })();
    App.Test = Test;    
})(App || (App = {}));

