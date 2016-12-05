(function () {
    'use strict';

    var app = angular.module('fb.dashboard');

    app.factory('accountBalanceRes', ['$resource', function ($resource) {
        return $resource('/api/accountBalance/:action', {}, {
            get: {
                method: 'GET'
            }
        });
    }]);
})();