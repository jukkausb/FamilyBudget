(function () {
    'use strict';

    var app = angular.module('fb.home');

    app.factory('tempRes', ['$resource', function ($resource) {
        return $resource('/api/income');
    }]);
})();