(function () {
    'use strict';

    var app = angular.module('fb.core');

    app.config([
        '$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push('loadingStatusInterceptor');
        }
    ]);

    app.directive('progressSpinner', ['$http', '$window', '$timeout', '$rootScope', function ($http, $window, $timeout, $rootScope) {
            return {
                templateUrl: '/Scripts/app/templates/progress-spinner.tpl.html',
                replace: true,
                link: function (scope, element) {
                    scope.showRefreshButton = false;
                    scope.showProgress = false;
                    var timerPromises = [];
                    var timerRefreshPromise = null;

                    function hide() {
                        scope.showRefreshButton = false;
                        var timer = timerPromises.pop();
                        $timeout.cancel(timer);
                        $timeout.cancel(timerRefreshPromise);
                        timerRefreshPromise = null;
                        scope.showProgress = false;
                        $rootScope.$broadcast('isInProgress', false);
                    };

                    function showRefresh() {
                        scope.showRefreshButton = true;
                        timerRefreshPromise = null;
                    }

                    function show(e, timeout) {
                        timeout = (timeout && !isNaN(timeout)) ? timeout : 400;
                        timerPromises.push($timeout(function () {
                            scope.showProgress = true;
                            $rootScope.$broadcast('isInProgress', true);
                            //if (e && e.name && e.name === '$locationChangeStart') {
                            //    $timeout(function () {//sometimes we get navigation events without a completion counterpart
                            //        if ($http.pendingRequests.length === 0) {
                            //            hide();
                            //        }
                            //    }, 1000);
                            //}
                            if (!timerRefreshPromise) {
                                timerRefreshPromise = $timeout(function () {
                                    showRefresh();
                                }, 12000);
                            }
                        }, timeout));
                    };

                    function showNow() {
                        show(null, 0);
                    }

                    scope.refresh = function () {
                        scope.showRefreshButton = false;
                        location.reload();
                    }

                    scope.$on('loadingStatusActive', show);
                    scope.$on('$locationChangeStart', show);
                    scope.$on('$stateChangeStart', showNow);
                    scope.$on('loadingStatusInactive', hide);
                    scope.$on('$stateChangeSuccess', hide);
                    scope.$on('$locationChangeSuccess', hide);
                    scope.$on('errorMessageTimeOut', showRefresh);
                    //angular.element($window).bind('offline', hide);

                    hide();

                    scope.$on('$destroy', function () {
                        $timeout.cancel(timerPromises);
                    });
                }
            };
        }
    ]);

    app.factory('loadingStatusInterceptor', ['$q', '$rootScope', '$timeout', function ($q, $rootScope, $timeout) {
            var activeRequests = 0;
            var started = function (doBroadcast) {
                if (doBroadcast) {
                    $timeout(function () {
                        $rootScope.$broadcast('loadingStatusActive');
                    });
                }
                activeRequests++;
            };
            var ended = function () {
                activeRequests--;
                if (activeRequests === 0) {
                    $timeout(function() {
                        $rootScope.$broadcast('loadingStatusInactive');
                    });
                    
                }
            };
            return {
                request: function (config) {
                    started(config.url.indexOf('.html') < 0);
                    return config || $q.when(config);
                },
                response: function (response) {
                    ended();
                    return response || $q.when(response);
                },
                responseError: function (rejection) {
                    ended();
                    return $q.reject(rejection);
                }
            };
        }
    ]);
})();