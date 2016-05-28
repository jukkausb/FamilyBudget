(function () {
    "use strict";
    angular.module('fb.home').controller('TestCtrl', ['$scope', '$http', 'fares', function ($scope, $http, fares) {

        console.log(fares);

    }]);
})();