angular.module("mainPage").component("mainPage", {

        templateUrl: 'app/partials/main-page/main-page.template.html',
        controllerAs: 'MainPage',

        controller: function MainPageController(FilmsService, $location, UsersService, ListsService, $resource, growl,
        localStorageService, usSpinnerService) {

            var _roles = [];
            var ctrl = this;

            ctrl.text = "";
            ctrl.IDFilm = 0;
            ctrl.ShowList = false;
            ctrl.button = true;
            ctrl.voteLists = [];
            ctrl.currentIndexList = 0;
            ctrl.currentIndexFilm = 0; 

            ctrl.TopLists = ListsService.ShowTopLists.query(
                function(response){
                    
                    var spinner = angular.element( document.querySelector( '#spinner' ) );
                    spinner.addClass('hidden');
                    
                    for(let i = 0; i < response.length; i++){
                        let _listOfFilms = [];
                        let _list = {};
                        _list.id = response[i].id;
                        for(let j = 0;  j < response[i].films.length; j++){
                            let _film = {};
                            _film.id = response[i].films[j].id
                            _film.votes = response[i].films[j].votes;
                            _film.isVoted = response[i].films[j].isVoted;
                            _listOfFilms.push(_film);
                        }
                        _list.films = _listOfFilms;
                        ctrl.voteLists.push(_list);
                    }
            });

            if(localStorageService.get('isLogged') == true && localStorageService.get('getUserData') == true){

                UsersService.ObtainLoggedUserInfo.get(
                function (response) {
                    localStorageService.set('userName', response.userName);
                    localStorageService.set('getUserData', false);
                    _roles = response.roles;

                    if(_roles.indexOf("Admin") > -1){
                        localStorageService.set('isAdmin', true);
                        ctrl.isAdmin = localStorageService.get('isAdmin');
                    }
                    else if(_roles.indexOf('Admin') == -1 ){
                        localStorageService.set('isAdmin', false);
                        ctrl.isAdmin = localStorageService.get('isAdmin');
                    }

                    if(_roles.indexOf('Moderator') > -1){
                        localStorageService.set('isModerator', true);
                        ctrl.isModerator = localStorageService.get('isModerator');
                    }

                    else if(_roles.indexOf('Moderator') == -1){
                        localStorageService.set('isModerator', false);
                        ctrl.isModerator = localStorageService.get('isModerator')
                    }
                })
            }
            ctrl.CloseModal = function () {
            //  ctrl.openModal = false;
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
               

            ctrl.isVoted = function (listIndex, filmIndex) {
                return true;
            }

            ctrl.Vote = function (film, list) {
                 
                let _currentList = {};
                if(!ctrl.isLogged){
                 //   ctrl.openModal = true;
                    return film;
                }else{
                    
                    for(let i = 0; i  < ctrl.voteLists.length; i ++){
                        if(ctrl.voteLists[i].id == list.id){
                            ctrl.currentIndexList = i;
                            _currentList = ctrl.voteLists[i];
                            break;
                        }
                    }
                    

                    for(let i = 0; i  < _currentList.films.length; i ++){
                        
                        if(_currentList.films[i].id == film.id){
                            if(ctrl.voteLists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].isVoted){
                                ctrl.currentIndexFilm = i;
                                ctrl.voteLists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes = ctrl.voteLists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes - 1;
                                ctrl.voteLists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].isVoted = false;
                                UsersService.RateFilm.save({FilmId: film.id, ListId: list.id});
                            return film;
                        }
                                else{
                                ctrl.currentIndexFilm = i;
                                
                                ctrl.voteLists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes = ctrl.voteLists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].votes + 1;
                                ctrl.voteLists[ctrl.currentIndexList].films[ctrl.currentIndexFilm].isVoted = true;
                                UsersService.RateFilm.save({FilmId: film.id, ListId: list.id});
                                 return film;
                            }
                        }
                    }
                }
            }                
                        
                

                    
            
            
            ctrl.foo = function(listId, index){
                let _films = [];
                for(let i = 0; i < ctrl.TopLists.length; i++){
                    if(ctrl.TopLists[i].id == listId){
                        _films = angular.copy(ctrl.TopLists[i].films);
                        
                        break;
                    }
                }
                return _films[index];
            }
            ctrl.lastFilm = function (listId, index) {
                let _films = [];
                for(let i = 0; i < ctrl.TopLists.length; i++){
                    if(ctrl.TopLists[i].id == listId){
                        _films = angular.copy(ctrl.TopLists[i].films);
                        break;
                    }
                }
                return (_films.length - 1);
            }
            ctrl.returnVotes = function (listId, index) {
                let _films = [];
                
                for(let i = 0; i < ctrl.TopLists.length; i++){
                    if(ctrl.TopLists[i].id == listId){
                        return ctrl.voteLists[i].films[index].votes;
                }
                }
            }
            
            ctrl.ShowCreator = function (film) {

              if(ctrl.isLogged){

                let _newFilm = {};
                let _lengthOfListCreator = ctrl.listCreator.length;
                let _addedFilm = false;

                //ctrl.openModal = false;
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

                    if (ctrl.showCreator == 'none') {

                        ctrl.contentCreator = false;
                        ctrl.changeVisibilityCreator({
                            $event: {
                                showCreator: 'block'
                            }
                        })
                    }
              }else{
              //  ctrl.openModal = true;
              }

            }
            
        },
        bindings: {
            isLogged: '=',
            isAdmin: '=',
            isModerator: '=',
            listCreator: '=',
            contentCreator: '=',
            showCreator: '<',
            openModal: '=',
            changeVisibilityCreator: '&',
        }

})