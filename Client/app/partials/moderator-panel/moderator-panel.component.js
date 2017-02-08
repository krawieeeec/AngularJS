angular.module('moderatorPanel').component('moderatorPanel', {

    templateUrl: 'app/partials/moderator-panel/moderator-panel.template.html',
    controllerAs: 'Moderator',
    controller: function ModeratorPanelController ($state,FilmsService,ListsService, localStorageService, growl) {

        ctrl = this;

        ctrl.isModerator = localStorageService.get('isModerator');

        if(!ctrl.isModerator){
          $state.go("main")
        }



        ctrl.AllLists = ListsService.ShowAllLists.query();


        ctrl.DeleteList = function(id){
           var spinner1 = angular.element( document.querySelector( '#spinner1' ) );
           spinner1.removeClass('hidden')
          ListsService.CrudRequestOnDataLists.delete({id: id},
          function (response) {
           spinner1.addClass('hidden')
            growl.success('Pomyślnie usunięto listę',{title: 'Sukces!'});
          }, function (response) {
            growl.error('Lista nie istnieje',{title: 'Błąd'});
           spinner1.addClass('hidden')
            });

        }


        ctrl.ChangeNameList = function(id, name){
           var spinner2 = angular.element( document.querySelector( '#spinner2' ) );
           spinner2.removeClass('hidden')
          ListsService.CrudRequestOnDataLists.update({id: id},{Name: name.$modelValue},
          function (response) {

            spinner2.addClass('hidden')
            growl.success('Pomyślnie zmieniono nazwę',{title: 'Sukces!'});
          }, function (response) {
            growl.error('Jakiś błąd',{title: 'Błąd'});
            spinner2.addClass('hidden')
            });

        }


    }, bindings:{
        isModerator: '<'
    }


})
