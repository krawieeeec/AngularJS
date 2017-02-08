angular.module('listRanking').component('listRanking', {

    templateUrl: 'app/partials/list-ranking/list-ranking.template.html',
    controllerAs: 'ListRanking',
    controller: function ListRankingController (ListsService) {

        ctrl = this;
        var spinner = angular.element( document.querySelector( '#favspinner' ) );
        spinner.removeClass('hidden')
        this.allLists = ListsService.CrudRequestOnDataLists.query(function(response){
          spinner.addClass('hidden')
        });
    }
})
