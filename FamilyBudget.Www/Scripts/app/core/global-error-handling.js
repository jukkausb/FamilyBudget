(function () {
    'use strict';

    var app = angular.module('fb.core');

    //app.config(['$httpProvider', function ($httpProvider) {
    //    $httpProvider.interceptors.push('timeoutHttpIntercept');
    //    $httpProvider.interceptors.push('errorHandlerHttpInterceptor');
    //}]);

    //app.factory('timeoutHttpIntercept', ['$q', '$rootScope', function ($rootScope, $q) {
    //    return {
    //        'request': function (config) {
    //            config.timeout = 60000;
    //            return config;
    //        }
    //    };
    //}]);

    //app.factory('errorHandlerHttpInterceptor', ['$q', '$rootScope', function ($q, $rootScope) {
    //    function showError(error, unrecoverable) {
    //        //var messageModel = error.data.exceptionDetails || error.data;
    //        //if (messageModel && (messageModel.hasMessage || messageModel.Message)) {
    //        //    $rootScope.$broadcast(unrecoverable ? 'errorMessageUnrecoverable' : 'errorMessageShow', messageModel);
    //        //}
    //    }

    //    function showStandardError(error) {
    //        showError(error);
    //    }

    //    function showUnrecoverableError(error) {
    //        showError(error, true);
    //    }

    //    return {
    //        response: function (response) {
    //            //if (response.data && response.data.messageModel) {
    //            //    var items = _.map(response.data.messageModel.messageItemList, function (item) {
    //            //        return item.text;
    //            //    });

    //            //    $rootScope.$broadcast('showInfoMessage', { items: items });
    //            //}
    //            return response || $q.when(response);
    //        },
    //        responseError: function (errorResponse) {
    //            // $http example
    //            // $http.get('/api/something', { custom400handling: true });
    //            if (errorResponse.status == 0) {
    //                $rootScope.$broadcast('errorMessageTimeOut');
    //            }
    //            if (errorResponse.status >= 400 && errorResponse.status < 500) {
    //                if (!errorResponse.config.custom400handling) {
    //                    showStandardError(errorResponse);
    //                }
    //            } else if (errorResponse.status >= 500) {
    //                if (!errorResponse.config.custom500handling) {
    //                    showUnrecoverableError(errorResponse);
    //                }
    //            }
    //            return $q.reject(errorResponse);
    //        }
    //    };
    //}]);
})();