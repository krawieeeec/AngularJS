'use strict';
angular.module("moodies").factory('OAuth2', function ($http, $q, $location, $state, $window, localStorageService, $rootScope, StoreLoggedState) {

    var serviceBase = 'http://moodies.pl/';
    var authServiceFactory = {};
    var _isLogged;

    var _login = function (login, password) {

        var data = "grant_type=password&username=" + login + "&password=" + password;

        return $http.post(serviceBase + 'api/token', data, {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).success(function (response) {

            localStorageService.set('BearerToken', response.access_token);
            localStorageService.set('IsLogged', true);
            alert("User has logged.");
            _isLogged = true;
            StoreLoggedState.set(true);
            $state.go('main');
           // $window.location.reload();
           // $location.url('file:///C:/Users/X/Desktop/GIT%20REPO/moodies.pl/Client/index.html#/main');
           //$state.go('main');

            
        }).error(function (err, status) {
            alert("User hasn't logged.");

        });

    };

    var _logout = function ($localstorage) {
        localStorageService.set('IsLogged', false);
        localStorageService.set('BearerToken', null);
    };

    authServiceFactory.Login = _login;
    authServiceFactory.Logout = _logout;
    authServiceFactory.isLogged = _isLogged; 
    return authServiceFactory;
});