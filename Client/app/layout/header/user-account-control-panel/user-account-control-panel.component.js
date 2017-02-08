angular.module('userAccountControlPanel').component('userAccountControlPanel', {


    templateUrl:'app/layout/header/user-account-control-panel/user-account-control-panel.template.html',
    controllerAs: 'UserControlPanel',
    controller: function UserAccountControlPanelController (localStorageService) {

        var ctrl = this;

        ctrl.isModerator = localStorageService.get('isModerator');
        ctrl.isAdmin = localStorageService.get('isAdmin');

    }, bindings: {
        isLogged: '<',
        isAdmin: '<',
        isModerator: '<'
    }
})
