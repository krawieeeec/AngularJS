angular.module('favoriteUserList').component('favoriteUserList', {

    templateUrl: 'app/partials/favorite-user-list/favorite-user-list.template.html',
    controllerAs: 'FavoriteUserList',
    controller: function FavoriteUserListController ($resource, UsersService, localStorageService, ListsService, growl) {


        let _baseUrl = "http://www.moodies.pl/";
        ctrl = this;

        if(localStorageService.get('isLogged')){
            var spinner = angular.element( document.querySelector( '#favspinner' ) );
            spinner.removeClass('hidden')
            ctrl.userFavouritesLists = UsersService.GetFavouritesLists.query({userName: localStorageService.get('userName')}, function(resp){
              if (resp.length == 0){
                spinner.addClass('hidden');
                var noFav = angular.element( document.querySelector( '#noFav' ) );
                noFav.removeClass('hidden');
              }else{
                ctrl.List = ListsService.CrudRequestOnDataLists.get({id: resp[0].id}, function(response){
                  spinner.addClass('hidden');
              });
              }
            });

        }

        ctrl.ShowList = function (fav) {
          var spinner = angular.element( document.querySelector( '#favspinner' ) );
          spinner.removeClass('hidden')
          ctrl.List = ListsService.CrudRequestOnDataLists.get({id: fav.id}, function(response){
          spinner.addClass('hidden');
          });
        }

        ctrl.RemoveFavorite = function(id){
          var link = angular.element( document.querySelector( '#link'+id ) );
          link.toggle( "slide" );
          var list = angular.element( document.querySelector( '#list'+id ) );
          list.slideUp();
          UsersService.RemoveFromFavorite.delete({listId: id},function(response){
            growl.success('Pomyślnie usunięto do ulubionych',{title: 'Sukces!'});
            ctrl.List = undefined;
          });
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
    bindings:{
        isLogged: '<'
    }

})
