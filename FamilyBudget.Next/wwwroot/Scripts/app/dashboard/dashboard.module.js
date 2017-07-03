(function () {
    'use strict';

    var app = angular.module('fb.dashboard', [
        'ngResource',
        'ngAnimate',
        'ui.router',
        'fb.core'
    ]);

    app.config(['$stateProvider', '$urlRouterProvider', '$urlMatcherFactoryProvider', function ($stateProvider, $urlRouterProvider, $urlMatcherFactory) {
        $urlRouterProvider.otherwise('/overview');
        $urlMatcherFactory.caseInsensitive(true);

        $stateProvider
            .state('dashboard', {
                templateUrl: 'Scripts/app/dashboard/dashboard.tpl.html'
            })
            .state('dashboard.overview', {
                url: '/overview',
                views: {
                    'maincontent': {
                        templateUrl: 'Scripts/app/dashboard/overview/dashboard.overview.tpl.html',
                        controller: 'DashboardOverviewCtrl',
                        controllerAs: 'vm',
                        resolve: {
                            accountBalanceModelData: ['accountBalanceRes', function (accountBalanceRes) {
                                return accountBalanceRes.get().$promise;
                            }]
                        }
                    }
                }
            });
    }]);
})();
