angular.module("userRegister").component("userRegister", {

    templateUrl: 'app/partials/user-register/user-register.template.html',
    controllerAs: 'UserRegister',

    controller: function UserRegisterController($resource, $timeout, $state, UsersService) {

        var _baseUrl = 'http://moodies.pl';
        var ctrl = this;
        //Use ngBind because of if template is raw before compiling anuglar you don't see double curly markup.
        //Model for dataBinding used in registering new user.

        ctrl.newUser = {
          "UserName": "",
          "Email": "",
          "Password": "",
          "ConfirmPassword": ""
        }

        ctrl.ClearData = function () {
            ctrl.newUser = {
              "UserName": "",
              "Email": "",
              "Password": "",
              "ConfirmPassword": ""
            }
                }

        ctrl.UserRegister = function() {
          var spinner = angular.element( document.querySelector( '#favspinner' ) );
          spinner.removeClass('hidden')

            let _registerNewUser = $resource(_baseUrl + '/api/accounts/register');

            _registerNewUser.save({UserName: ctrl.newUser.UserName, Email: ctrl.newUser.Email, Password: ctrl.newUser.Password,
                ConfirmPassword: ctrl.newUser.ConfirmPassword},
                function (response) {
                     spinner.addClass('hidden');
                     ctrl.showSuccess = true;
                     var form = angular.element( document.querySelector( '#form' ) );
                     form.remove();

                }, function (response) {
                  spinner.addClass('hidden');
                    switch(response.data.message){
                        case 'USERNAME_ALREADY_TAKEN':
                            ctrl.userNameExists = true;
                            ctrl.newUser.UserName = "";
                            $timeout(function () {
                                ctrl.userNameExists = false;
                            }, 5000);
                            break;
                        case 'DIFFERENT_PASSWORDS':
                            ctrl.newUser.Password = "";
                            ctrl.newUser.ConfirmPassword = "";
                            ctrl.differentPasswords = true;
                            $timeout(function () {
                                ctrl.differentPasswords = false;
                            }, 5000);
                            break;
                        case 'EMAIL_ALREADY_TAKEN':
                            ctrl.emailExists = true;
                            ctrl.newUser.Email = "";
                            $timeout(function () {
                                ctrl.emailExists = false;
                            }, 5000);
                            break;
                        //'UNABLE_REGISTER' it mean other problem than rest cases meant before.
                        default:
                        ctrl.unableRegister = true;
                            $timeout(function () {
                                ctrl.unableRegister = false;
                                ctrl.ClearData();
                            }, 5000);

                    }

                    // body
                })

        }

    }
})
