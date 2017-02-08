angular.module('retriveUserAccount').component('retriveUserAccount', {

    templateUrl: 'app/partials/retrive-user-account/retrive-user-account.template.html',
    controllerAs: 'RetriveUserAccount',
    controller: function RetriveUserAccountController ($resource, $state, $timeout, UsersService) {
        
        var _baseUrl = 'http://moodies.pl';
        var ctrl = this;

        ctrl.retriveUserAccount = {
            Email: ""
        }

        ctrl.RetriveAccount = function  () {
            
            let _retriveUserAccount = $resource(_baseUrl + '/api/accounts/ForgotPassword');

            _retriveUserAccount.save({Email: ctrl.retriveUserAccount.Email}, 
                function (response) {
                    ctrl.showSuccess = true;
                        $timeout(function () {
                            ctrl.showSuccess = false;
                            ctrl.retriveUserAccount.Email = "";
                            $state.go('main');
                        }, 8000);
            }, function (response) {
                    switch(response.data.message){
                        case 'ACCOUNT_NOT_CONFIRMED':
                            ctrl.userAccountNotConfirmed = true;
                            $timeout(function () {
                                ctrl.userAccountNotConfirmed = false;
                                ctrl.retriveUserAccount.Email = "";
                            }, 5000);
                            break;
                        case 'EMAIL_NOT_EXISTS':
                            ctrl.emailNotExists = true;
                            $timeout(function () {
                                ctrl.emailNotExists = false;
                                ctrl.retriveUserAccount.Email = "";
                            }, 5000);
                            break;
                        //'UNABLE_REGISTER' it mean other problem than rest cases meant before.
                        default:    
                        ctrl.unableRegister = true;
                            $timeout(function () {
                                ctrl.unableRegister = false;
                                ctrl.retriveUserAccount.Email = "";
                            }, 5000);


                    }
                }
            )
        }

        ctrl.ClearData = function () {
            ctrl.retriveUserAccount.Email = "";
        }
    }

})