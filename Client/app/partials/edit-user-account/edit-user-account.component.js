angular.module("editUserAccount").component("editUserAccount", {

    templateUrl: 'app/partials/edit-user-account/edit-user-account.template.html',
    controllerAs: 'EditUserAccount',
    controller: function EditUserAccountController($state, $timeout, UsersService) {

        var ctrl = this;

        ctrl.editUserAccount = {

            NewEmail: '',
            CurrentPassword: '',
            NewPassword: '',
            ConfirmNewPassword: ''

        }

        ctrl.ClearData = function () {

            ctrl.editUserAccount.NewEmail = "";
            ctrl.editUserAccount.CurrentPassword = "";
            ctrl.editUserAccount.NewPassword = "";
            ctrl.editUserAccount.ConfirmNewPassword = "";
            $state.go('main');
        }

        ctrl.ChangeUserEmail = function () {

            UsersService.ChangeEmail.save({NewEmail: ctrl.editUserAccount.NewEmail},
                function (response) {
                    ctrl.showSuccessChangeEmail = true;
                    var form = angular.element( document.querySelector( '#changingUserEmail' ) );
                    form.remove();
                }, function (response) {

                    switch(response.data.message){
                        case 'EMAIL_ALREADY_TAKEN':
                            ctrl.emailAlreadyTaken = true;
                            $timeout(function () {
                                ctrl.emailAlreadyTaken = false;
                                ctrl.editUserAccount.NewEmail = "";
                            }, 5000);
                            break;
                        case 'UNABLE_CHANGE_EMAIL':
                            ctrl.unableChangeEmail = true;
                            $timeout(function () {
                                ctrl.unableChangeEmail = false;
                                ctrl.editUserAccount.NewEmail = "";
                            }, 5000);
                            break;
                        //brakuje default bo nie działa tu ;///

                    }
                });
        }

        ctrl.ChangePassword = function  () {
            UsersService.ChangePassword.save({CurrentPassword: ctrl.editUserAccount.CurrentPassword, NewPassword: ctrl.editUserAccount.NewPassword,
                ConfirmNewPassword: ctrl.editUserAccount.ConfirmNewPassword},
                function (response) {
                    ctrl.showSuccessChangePassword = true;
                    ctrl.editUserAccount.CurrentPassword = "";
                    ctrl.editUserAccount.NewPassword = "";
                    ctrl.editUserAccount.ConfirmNewPassword = "";
                    var form = angular.element( document.querySelector( '#changingUserPassword' ) );
                    form.remove();
                }, function (response) {
                    switch(response.data.message){
                        case 'DIFFERENT_PASSWORDS':
                            ctrl.diffrentPasswords = true;
                            ctrl.editUserAccount.NewPassword = "";
                            ctrl.editUserAccount.ConfirmNewPassword = "";
                            $timeout(function () {
                                ctrl.diffrentPasswords = false;
                            }, 5000);
                            break;
                        case 'PASSWORD_TOO_SHORT':
                            ctrl.shortPassword = true;
                            ctrl.editUserAccount.NewPassword = "";
                            ctrl.editUserAccount.ConfirmNewPassword = "";
                            $timeout(function () {
                                ctrl.shortPassword = false;
                            }, 5000);
                            break;
                        case 'WRONG_PASSWORD':
                            ctrl.wrongPassword = true;
                            ctrl.editUserAccount.NewPassword = "";
                            ctrl.editUserAccount.ConfirmNewPassword = "";
                            $timeout(function () {
                                ctrl.wrongPassword = false;
                            }, 5000);
                            break;
                        case 'UNABLE_CHANGE_PASSWORD':
                            ctrl.unableChangePassword = true;
                            ctrl.editUserAccount.CurrentPassword = "";
                            ctrl.editUserAccount.NewPassword = "";
                            ctrl.editUserAccount.ConfirmNewPassword = "";
                            $timeout(function () {
                                ctrl.unableChangePassword = false;
                            }, 5000);
                            break;
                    }
                })
        }
    }

})
