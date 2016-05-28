(function () {
    "use strict";

    var app = angular.module('fb.home', [
        'ngResource',
        'ngAnimate',
        'fb.core',
        'ui.router'
    ]);

    app.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
        $urlRouterProvider.otherwise('/home/dashboard');
        $stateProvider
            .state('home', {
                templateUrl: '/Scripts/app/home/templates/home.tpl.html',
                controller: 'HomeCtrl',
                controllerAs: 'vm'
            })
            .state('home.dashboard', {
                url: '/home/dashboard',
                views: {
                    'maincontent': {
                        templateUrl: '/Scripts/app/home/templates/dashboard.tpl.html',
                        controller: 'DashboardCtrl',
                        controllerAs: 'vm'
                    }
                },
                resolve: {
                    homeTestData: ['tempRes', function (tempRes) {
                        return tempRes.get().$promise;
                    }
                    ]
                }
            })
            .state('home.test', {
                url: '/home/test',
                views: {
                    'maincontent': {
                        templateUrl: '/Scripts/app/home/templates/test.tpl.html',
                        controller: 'TestCtrl',
                        controllerAs: 'vm'
                    }
                },
                resolve: {
                    fares: ['tempRes', function (tempRes) {
                        return tempRes.get().$promise;
                    }
                    ]
                }
            })
            .state('home.abc', {
                url: '/home/abc',
                views: {
                    'maincontent': {
                        templateUrl: '/Scripts/app/home/templates/abc.tpl.html',
                        controller: 'AbcCtrl',
                        controllerAs: 'vm'
                    }
                },
                resolve: {
                    abc: ['tempRes', function (tempRes) {
                        return tempRes.get().$promise;
                    }
                    ]
                }
            })
        ;
    }
    ]);
})();