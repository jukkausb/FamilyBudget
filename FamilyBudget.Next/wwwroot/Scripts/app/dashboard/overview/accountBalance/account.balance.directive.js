(function () {
    "use strict";

    var app = angular.module("fb.dashboard");

    app.directive('accountBalanceList', function () {
        return {
            restrict: 'A',
            require: '^ngModel',
            scope: {
                ngModel: '='
            },
            templateUrl: 'Scripts/app/dashboard/overview/accountBalance/account.balance.tpl.html',
            controller: 'AccountBalanceListCtrl',
            controllerAs: 'vm'
        };
    });

    app.controller('AccountBalanceListCtrl', ['$scope', function ($scope) {
        var vm = this;
        vm.model = $scope.ngModel;
    }]);

})();