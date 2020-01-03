(function () {
    'use strict';

    var app = angular.module('fb.core');

    app.filter('numberSpaces', function () {
        return function (input) {
            return angular.isDefined(input) && input.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
        };
    });

})();