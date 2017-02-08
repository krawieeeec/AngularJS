//This service perform crud operations on lists from server.

angular.module("moodies").factory("ListsService", function ($resource) {

        let _baseUrl = "http://moodies.pl";
        let listService = {};

        let _crudRequestsOnDataLists = $resource(_baseUrl + '/api/lists/:id', { id: '@id'}, {

            'update': {
                method: 'PUT',
                params:{Name: '@name', FilmIds: '@isList'}
            }
        });

        let _showToplists = $resource( _baseUrl + '/api/lists/top');
        let _showAllLists = $resource( _baseUrl + '/api/lists/listsTitles');
        let _rateList = $resource( _baseUrl + '/api/lists/rate');



        listService.CrudRequestOnDataLists = _crudRequestsOnDataLists;
        listService.ShowTopLists = _showToplists;
        listService.ShowAllLists = _showAllLists;
        listService.RateList = _rateList;

        return listService;
    })
