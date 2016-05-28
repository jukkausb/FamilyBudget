(function () {
    "use strict";
    angular.module('fb.core').controller('CoreCtrl', ['$scope', '$http', function ($scope, $http) {
        $scope.isBusy = false;

        $scope.$on('isInProgress', function (event, isInProgress) {
            $scope.isBusy = isInProgress;
        });


    }]);
})();