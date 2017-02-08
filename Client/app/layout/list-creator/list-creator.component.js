angular.module('listCreator').component('listCreator', {

        templateUrl: 'app/layout/list-creator/list-creator.template.html',
        controllerAs: 'Creator',
        controller: function ListCreatorController(localStorageService, ListsService, $timeout, usSpinnerService) {

            var ctrl = this;
            ctrl.contentCreator = false;
            ctrl.listOfIdFilms = [];
            ctrl.showSuccess = false;
            ctrl.showError = false;
            ctrl.showSpinner = false;
            ctrl.disableButton = false;
            var spinner = angular.element( document.querySelector( '#spinner' ) );
            spinner.addClass('hidden-spinner-creator');
            //Methods
            ctrl.MinimalizeCreator = function (value) {

                if (ctrl.isLogged) {

                    if (value == true) {

                        ctrl.contentCreator = false;
                    } else
                        ctrl.contentCreator = true;

                }
            }

            ctrl.RemoveFilm = function (index) {

                ctrl.listCreator.splice(index, 1);
                ctrl.listOfIdFilms.splice(index, 1);

            }

            ctrl.ClearCreatorData = function () {

                ctrl.listCreator = [];
                // ctrl.nameList = null;
                ctrl.listOfIdFilms = [];

            }

            ctrl.CreateNewList = function () {

                ctrl.showSpinner = true;
                ctrl.showButtons = false;
                ctrl.disableButton = true;
                let _notFoundId = -1;

                if (ctrl.listCreator.length > 0) {

                    for (let i = 0; i < ctrl.listCreator.length; i++) {
                        if (ctrl.listOfIdFilms.indexOf(ctrl.listCreator[i].id) == _notFoundId)
                            ctrl.listOfIdFilms.push(ctrl.listCreator[i].id);
                        else
                            continue;
                        }
                }
                spinner.addClass('show');

                ListsService.CrudRequestOnDataLists.save({Name: ctrl.nameList, FilmIds: ctrl.listOfIdFilms},
                function (response) {

                     ctrl.showSpinner = false;
                     ctrl.showButtons = true;
                     ctrl.showSuccess = true;
                        $timeout(function () {
                            ctrl.showSuccess = false;
                            ctrl.disableButton = false;
                        }, 3000);
                        $timeout(function () {
                          ctrl.ChangeVisibilty();
                        }, 2000);



                }, function (error) {

                        ctrl.showButtons = true;
                        ctrl.showSpinner = false;
                        ctrl.showError = true;
                        $timeout(function () {
                            ctrl.showError = false;
                            ctrl.disableButton = false;
                        }, 3000);

                    });
            }

            ctrl.EditList = function () {
              ctrl.showSpinner = true;
              ctrl.showButtons = false;
              ctrl.disableButton = true;

                let _idFilms = [];
                for(let i = 0; i < ctrl.listCreator.length; i++){
                    _idFilms.push(ctrl.listCreator[i].id);
                }
                spinner.addClass('show');
                ListsService.CrudRequestOnDataLists.update({id:ctrl.idList},{Name: ctrl.nameList, FilmIds: _idFilms},
                function(response){
                  ctrl.showSpinner = false;
                  ctrl.showButtons = true;
                  ctrl.showSuccess = true;
                     $timeout(function () {
                         ctrl.showSuccess = false;
                         ctrl.disableButton = false;
                     }, 3000);
                     $timeout(function () {
                       ctrl.ChangeVisibilty();
                     }, 2000);

                });
            }

            ctrl.ChangeVisibilty = function () {
                ctrl.listCreator = [];
                ctrl.listOfIdFilms = [];
                ctrl.contentCreator = false;
                ctrl.editList = false;
                ctrl.nameList = "";
                ctrl.changeVisibilityCreator({
                    $event: {
                        showCreator: 'none'
                    }
                })
            }
        },
        bindings: {
            isLogged: '=',
            listCreator: '=',
            nameList: '=',
            contentCreator: '=',
            showCreator: '<',
            editList: '=',
            showButtons: '=',
            showSpinner: '=',
            idList: '=',
            changeVisibilityCreator: '&'
        }

    })
