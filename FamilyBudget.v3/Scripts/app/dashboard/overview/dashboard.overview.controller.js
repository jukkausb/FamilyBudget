(function () {
    'use strict';

    var app = angular.module('fb.dashboard');

    app.controller('DashboardOverviewCtrl', ['accountBalanceModelData', function (accountBalanceModelData) {
        var vm = this;
        vm.accountBalanceModel = accountBalanceModelData;
    }
    ]);
})();