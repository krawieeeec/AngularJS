angular.module('resultSearchBar').component('resultSearchBar', {

    templateUrl: 'app/layout/result-search-bar/result-search-bar.template.html',
    controllerAs: 'ResultSearchBar',
    controller: function ResultSearchBar ($resource, $stateParams, UsersService, growl,
        localStorageService, usSpinnerService, ListsService) {

        let _baseUrl = "http://www.moodies.pl/";
        ctrl = this;
        ctrl.result = $stateParams.result;
        ctrl.films = [];
        ctrl.lists = [];
        ctrl.currentIndexList = 0;
        ctrl.currentIndexFilm = 0; 

        let _searchingBar = $resource(_baseUrl + 'api/lists/search/:searchString', {searchString: '@searchString'});

            _searchingBar.get({searchString: $stateParams.result},
                function (response) {
                    var spinner1 = angular.element( document.querySelector( '#spinner1' ) );
                    spinner1.addClass('hidden');
                    var spinner2 = angular.element( document.querySelector( '#spinner2' ) );
                    spinner2.addClass('hidden');
                    
                    ctrl.films = response.films;
                    if(response.films==""){
                      var noFilms = angular.element( document.querySelector( '#noFilms' ) );
                      noFilms.removeClass('hidden');
                    }
                    
                    ctrl.lists = angular.copy(response.lists);
                    
                    if(response.lists==""){
                      var noLists = angular.element( document.querySelector( '#noLists' ) );
                      noLists.removeClass('hidden')
                    }
            });

                    ctrl.CloseModal = function () {
              var loginModal = angular.element( document.querySelector( '#loginModal' ) );
              loginModal.removeClass('open');
              loginModal.addClass('close');
            }


            ctrl.VoteListLike = function(list){
              if (ctrl.isLogged){
                if (list.voted == -1) {
                  var element = angular.element( document.querySelector( '#dislikesL'+list.id ) );
                  element.removeClass('isVoted');
                  list.disLikes = list.disLikes - 1;
                  list.voted = 0;
                }
                if(list.voted == 1){
                  var element = angular.element( document.querySelector( '#likesL'+list.id ) );
                  element.removeClass('isVoted');
                  list.likes = list.likes - 1;
                  list.voted = 0;
              }else{
                var element = angular.element( document.querySelector( '#likesL'+list.id ) );
                element.addClass('isVoted');
                list.likes = list.likes + 1;
                list.voted = 1;
              }
              ListsService.RateList.save({ListId: list.id, Rate : list.voted});
            }else{
              var loginModal = angular.element( document.querySelector( '#loginModal' ) );
              loginModal.removeClass('close');
              loginModal.addClass('open');
            }
            }

            ctrl.VoteListDislike = function(list){
              if (ctrl.isLogged){
                if (list.voted == 1) {
                  var element = angular.element( document.querySelector( '#likesL'+list.id ) );
                  element.removeClass('isVoted');
                  list.likes = list.likes - 1;
                  list.voted = 0;
                }

                if(list.voted == -1){
                  var element = angular.element( document.querySelector( '#dislikesL'+list.id ) );
                  element.removeClass('isVoted');
                  list.disLikes = list.disLikes - 1;
                  list.voted = 0;
              }else{
                var element = angular.element( document.querySelector( '#dislikesL'+list.id ) );
                element.addClass('isVoted');
                list.disLikes = list.disLikes + 1;
                list.voted = -1;
              }
              ListsService.RateList.save({ListId: list.id, Rate : list.voted});
            }else{
              var loginModal = angular.element( document.querySelector( '#loginModal' ) );
              loginModal.removeClass('close');
              loginModal.addClass('open');
            }
            }

            ctrl.AddToFavourites = function(list){
              if (ctrl.isLogged){
              UsersService.AddFavoriteList.save({ListId: list.id},
              function (response) {
                growl.success('Pomyślnie dodano do ulubionych',{title: 'Sukces!'});
              }, function (response) {
                switch(response.data.message){
                    case 'LIST_NOT_FOUND':
                        growl.error('Nie znaleziono listy',{title: 'Błąd'});
                        break;
                    case 'LIST_ALREADY_MARKED_AS_FAVOURITE':
                        growl.warning('Lista jest juz w ulubionych',{title: 'Uwaga!'});
                        break;
                    case 'UNABLE_ADD_LIST_TO_FAVOURITES':
                        growl.error('Nieznany błąd',{title: 'Błąd!'});
                        break;
                  }
                });
                }else{
                  var loginModal = angular.element( document.querySelector( '#loginModal' ) );
                  loginModal.removeClass('close');
                  loginModal.addClass('open');
                }

            }

            
            ctrl.ShowCreator = function (film) {

              if(ctrl.isLogged){

                let _newFilm = {};
                let _lengthOfListCreator = ctrl.listCreator.length;
                let _addedFilm = false;
                if (ctrl.isLogged) {
                    for (let i = 0; i < _lengthOfListCreator; i++) {
                        if (ctrl.listCreator[i].id == film.id) {
                            _addedFilm = true;
                            break;
                        }
                    }


                    if (!_addedFilm) {

                        _newFilm.name = film.name;
                        _newFilm.id = film.id;
                        ctrl.listCreator.push(_newFilm);
                        ctrl.lengthOfArrayIdFilms = ctrl.listCreator.length;

                    }
                }
                    if (ctrl.showCreator == 'none') {
                        ctrl.changeVisibilityCreator({
                            $event: {
                                showCreator: 'block'
                            }
                        })
                    }
              }else{
                var loginModal = angular.element( document.querySelector( '#loginModal' ) );
                loginModal.removeClass('close');
                loginModal.addClass('open');
              }


            }
            //Pagination
            ctrl.Vote = function (film, list) {
                 
                let _currentList = {};
                if(!ctrl.isLogged){
                   // ctrl.openModal = true;
                    return film;
                }else{
                    
                    for(let i = 0; i  < ctrl.lists.length; i ++){
                        if(ctrl.lists[i].id == list.id){
                            ctrl.currentIndexList = i;
                            _currentList = ctrl.lists[i];
                            break;
                        }
                    }
                    

                    for(let i = 0; i  < _currentList.films.length; i ++){
                        
                        if(_currentList.films[i].id == film.id){
                            if(ctrl.lists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].isVoted){
                                ctrl.currentIndexFilm = i;
                                ctrl.lists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes = ctrl.lists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes - 1;
                                ctrl.lists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].isVoted = false;
                                UsersService.RateFilm.save({FilmId: film.id, ListId: list.id});
                            return film;
                        }
                                else{
                                ctrl.currentIndexFilm = i;
                                
                                ctrl.lists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes = ctrl.lists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes + 1;
                                ctrl.lists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].isVoted = true;
                                UsersService.RateFilm.save({FilmId: film.id, ListId: list.id});
                                 return film;
                            }
                        }
                    }
                }
            }
            ctrl.foo = function(listId, index){
                let _films = [];
                

                for(let i = 0; i < ctrl.lists.length; i++){
                    if(ctrl.lists[i].id == listId){
                        _films = angular.copy(ctrl.lists[i].films);
                         
                        break;
                    }
                }
                return _films[index];
            }
            ctrl.lastFilm = function (listId, index) {
                let _films = [];
                for(let i = 0; i < ctrl.lists.length; i++){
                    if(ctrl.lists[i].id == listId){
                        _films = angular.copy(ctrl.lists[i].films);
                        break;
                    }
                }
                return (_films.length - 1);
            }

    },
        bindings: {
            isLogged: '=',
            listCreator: '=',
            showCreator: '<',
            lengthOfArrayIdFilms: '=',
            changeVisibilityCreator: '&'
        }


})
