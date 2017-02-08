angular.module('ownUserList').component('ownUserList', {

    templateUrl: 'app/partials/own-user-list/own-user-list.template.html',
    controllerAs: 'OwnUserList',
    controller: function OwnUserListController ($resource, UsersService, localStorageService, ListsService) {

      let _baseUrl = "http://www.moodies.pl/";
      ctrl = this;

      if(localStorageService.get('isLogged')){
          ctrl.ownUserLists = UsersService.GetOwnLists.query({userName: localStorageService.get('userName')},
            function(response){
          });

      }

      ctrl.ShowList = function (own) {
          var spinner = angular.element( document.querySelector( '#spinner'+own.id ) );
          spinner.removeClass('hidden')
          ctrl.List = ListsService.CrudRequestOnDataLists.get({id: own.id}, function(response){
          spinner.addClass('hidden');
        });
      }

      ctrl.DeleteList = function(listId){
          var lista = angular.element( document.querySelector( '#list'+listId ) );
          lista.toggle( "slide" );
          ListsService.CrudRequestOnDataLists.delete({id: listId},
            function(response){}, function() {
              lista.toggle( "slide" );}
            );
      }

      ctrl.EditList = function (nameList, listId) {

        let _filmExists = false;
        ctrl.listCreator = [];
        ctrl.editList = true;
        ctrl.showButtons = false;
        ctrl.showSpinner = true;
        ctrl.idList = listId;
        ListsService.CrudRequestOnDataLists.get({id: listId},
          function (response) {
            ctrl.showSpinner = false;
            ctrl.showButtons = true;
            angular.forEach(response.films, function (value, key)  {
              let _film = {};
              for(let i = 0; i < ctrl.listCreator.length; i++){
                if(value.id == ctrl.listCreator[i].id){
                  _filmExists = true;
                  break;
                }
              }
              if(!_filmExists){
                _film.id = value.id;
                _film.name = value.name;
                this.push(_film);
              }
              _filmExists = false;
            }, ctrl.listCreator)
            ctrl.nameList = nameList;
            ctrl.listCreator.sort();
        });

        if (ctrl.showCreator == 'none') {
                        ctrl.contentCreator = false;
                        ctrl.changeVisibilityCreator({
                            $event: {
                                showCreator: 'block'
                            }
                        })
                    }
      }
      ctrl.Vote = function (film, list) {
        if(!film.isVoted){
          var element = angular.element( document.querySelector( '#ratingL'+list.id+"F"+film.id ) );
          element.addClass('isVoted');
          film.votes = film.votes + 1;
          film.isVoted = true;
      }else{
          var element = angular.element( document.querySelector( '#ratingL'+list.id+"F"+film.id ) );
          element.removeClass('isVoted');
          film.votes = film.votes - 1;
          film.isVoted = false;
      }
        UsersService.RateFilm.save({FilmId: film.id, ListId: list.id},
        function (response) {
        }, function (error) {
          if(!film.isVoted){
            var element = angular.element( document.querySelector( '#ratingL'+list.id+"F"+film.id ) );
            element.addClass('isVoted');
            film.votes = film.votes + 1;
            film.isVoted = true;
        }else{
            var element = angular.element( document.querySelector( '#ratingL'+list.id+"F"+film.id ) );
            element.removeClass('isVoted');
            film.votes = film.votes - 1;
            film.isVoted = false;
        }
            });

      }
    },
        bindings: {
            isLogged: '=',
            listCreator: '=',
            contentCreator: '=',
            showCreator: '=',
            nameList: '=',
            editList: '=',
            showButtons: '=',
            showSpinner: '=',
            idList: '=',
            changeVisibilityCreator: '&'
        }

})
