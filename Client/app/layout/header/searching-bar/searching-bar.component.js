angular.module('searchingBar').component('searchingBar', {

    templateUrl: 'app/layout/header/searching-bar/searching-bar.template.html',
    controllerAs: 'SearchingBar',
    controller: function SearchingBarController (localStorageService, $resource, usSpinnerService, $state, $timeout) {
        
        
        ctrl = this;

        ctrl.searchedTerm = "";

        ctrl.Send = function (_result) {
            let _query = angular.copy(_result);
            
            ctrl.searchedTerm = "";
            $state.go('resultSearchBar', {result: _query})
                        
        }

        }
})



        