angular.module('showDetailsFilm').component('showDetailsFilm', {

        templateUrl: 'app/partials/show-details-film/show-details-film.template.html',
        controllerAs: 'Film',
        controller: function ShowDetailsFilmController($stateParams, FilmsService) {

            var ctrl = this;

            ctrl.IDFilm = 0;
            ctrl.ShowList = false;
            ctrl.DetailsOfFilm = FilmsService.CrudRequestsOnDataFilms.get({id: $stateParams.filmId});

            ctrl.ShowCreator = function (film) {

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



            }

    },
        bindings: {
            isLogged: '<',
            listCreator: '=',
            showCreator: '<',
            lengthOfArrayIdFilms: '=',
            changeVisibilityCreator: '&',
        }
})
