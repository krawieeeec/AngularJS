//This service perform crud operations on films from server.

angular.module("moodies").factory("FilmsService", function($resource){

    let filmService = {}

    let _crudRequestsOnDataFilms = $resource('http://moodies.pl/api/films/:id', {id: '@id'}, {
        'update': {method: 'PUT'}
    });


    filmService.CrudRequestsOnDataFilms = _crudRequestsOnDataFilms

    return filmService;

    


})