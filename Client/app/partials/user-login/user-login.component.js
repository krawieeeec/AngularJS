angular.module("userLogin").component("userLogin", {

        templateUrl: 'app/partials/user-login/user-login.template.html',
        controllerAs: "AuthorizationUser",
        controller: function UserLoginController($location, $http, $state, $timeout, localStorageService, growl) {


            var ctrl = this;

            this.isDisabled = false;

            ctrl.UserLogin = {
                //Thanks to this fake data you can login on serwer as user.
                Login: "",
                Password: ""
            }

            //Function calling method resposible for logic of authorization user.
            ctrl.AuthorizationRequest = function (login, password) {
              this.isDisabled = true;
              var spinner = angular.element( document.querySelector( '#favspinner' ) );
              spinner.removeClass('hidden');

                let serviceBase = 'http://moodies.pl/';
                let data = "grant_type=password&username=" + login + "&password=" + password;

                $http.post(serviceBase + 'api/token', data, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }
                    })
                    .then(function successCallback(response) {
                      spinner.addClass('hidden');
                      localStorageService.set('BearerToken', response.data.access_token);
                      localStorageService.set('isLogged', true);
                      localStorageService.set('getUserData', true);

                      ctrl.onViewChange({
                          $event: {
                              isLogged: true
                          }
                      })


                      $timeout(function () {
                        $state.go('main');
                      },1000);

                        }, function errorCallback(response) {
                          spinner.addClass('hidden');
                          this.isDisabled = false;
                            ctrl.UserLogin.Login = "";
                            ctrl.UserLogin.Password = "";
                            ctrl.onViewChange({
                              $event: {
                                isLogged: false
                              }
                            })
                            growl.error('Błędne hasło lub login',{title: 'Błąd'});
                          });
            };
        },
        bindings: {

            onViewChange: '&'
        }
    });
