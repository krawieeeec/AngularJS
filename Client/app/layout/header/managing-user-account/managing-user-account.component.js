angular.module('managingUserAccount').component('managingUserAccount', {

    templateUrl: 'app/layout/header/managing-user-account/managing-user-account.template.html',
    controllerAs: 'UserAccount',
    controller: function ManagingUserAccountController(OAuth2, $window, localStorageService, $state) {

        var ctrl = this;

        ctrl.Logout = function () {

            ctrl.isLogged = false;
            OAuth2.Logout();
            localStorageService.set('isLogged', false);
            localStorageService.set('BearerToken', null);
            localStorageService.set('isAdmin', false);
            localStorageService.set('isModerator', false);
            ctrl.viewOnChange({
                    $event: {
                        isLogged: 'false'
                    }
                })
                $window.location.reload();
                $state.go('main', {}, {reload: true});

        }


    },
    bindings: {
        isLogged: '<',
        viewOnChange: '&'
    }

})
