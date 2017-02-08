angular.module('adminPanel').component('adminPanel', {

    templateUrl: 'app/partials/admin-panel/admin-panel.template.html',
    controllerAs: 'Admin',
    controller: function AdminPanelController ($state, UsersService, localStorageService, growl) {

      ctrl = this;
      ctrl.isAdmin = localStorageService.get('isAdmin');
      ctrl.disableButton = false;
      ctrl.userRole = {
        moderator: "",
        admin: ""
      }
      var spinner1 = angular.element( document.querySelector( '#spinner1' ) );
      var spinner2 = angular.element( document.querySelector( '#spinner2' ) );

      ctrl.AllUsers = UsersService.GetAllUsers.query();
      ctrl.DeleteUser = function(userName){

        spinner1.removeClass('hidden')
        ctrl.disableButton = true;
        UsersService.DeleteUser.delete({userName: userName},
          function (response) {
            spinner1.addClass('hidden')
            ctrl.disableButton = false;
            growl.success('Pomyślnie usunięto uzytkownika',{title: 'Sukces!'});
            }, function (response) {
              ctrl.disableButton = false;
              growl.error('Uzytkownik nie istnieje',{title: 'Błąd'});
              spinner1.addClass('hidden')
          });

        }

        ctrl.SetRole = function(userName, role){

          let _userRoles = [];
          spinner2.removeClass('hidden')
          ctrl.disableButton = true;
          if(role.admin == true){
            ctrl.userRole.admin = "Admin";
            _userRoles.push(ctrl.userRole.admin);
          }

          if(role.moderator == true){
            ctrl.userRole.moderator = "Moderator"
            _userRoles.push(ctrl.userRole.moderator);
          }

          UsersService.SetRole.save({}, {UserName: userName, Roles: _userRoles},
          function (response) {
            spinner2.addClass('hidden')
            ctrl.disableButton = false;
            growl.success('Nadano rolę',{title: 'Sukces!'});
            }, function (response) {
              ctrl.disableButton = false;
              spinner2.addClass('hidden')
              switch(response.data.message){
                    case 'ROLE_NOT_FOUND':
                        growl.error('Podana rola nie istnieje!',{title: 'Błąd'});
                        break;
                    case 'ROLE_ALREADY_ADDED_FOR_USER':
                        growl.warning('Wybarane role już istnieją',{title: 'Uwaga!'});
                        break;
                    case 'UNABLE_ADD_ROLE':
                        growl.error('Nieznany błąd',{title: 'Błąd!'});
                        break;
                  }
              
              });

        }

    },
    bindings: {
        isAdmin: '<'
    }


})
