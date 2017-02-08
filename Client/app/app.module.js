//This command cause compiler will throw errors which were acceptable thorugh older compiler in
// older version JS's scripts. For more information visit this link:
// http://www.w3schools.com/js/js_strict.asp

angular.module("moodies", ['ui.router', 'ngResource', 'ngAnimate', 'navbar', 'managingUserAccount', 'mainLogo', 'searchingBar', 'footerOfLayout',
    'LocalStorageModule', 'mainPage', 'editUserAccount', 'userRegister', 'userLogin', 'showDetailsFilm', 'listRanking', 'listCreator',
    'userAccountControlPanel', 'favoriteUserList', 'ownUserList', 'retriveUserAccount', 'angular-growl', 'angularSpinner', 'adminPanel', 'moderatorPanel',
    'ui.select2', 'resultSearchBar'

])


.config(function ($stateProvider, $urlRouterProvider, $httpProvider, $rootScopeProvider, growlProvider) {
    growlProvider.globalTimeToLive(3000);
    growlProvider.globalPosition('top-left');
    //NOTES!
    //During fetching params, properties 'component' or 'controller don't' have any value or meaning when we pass params to component.
    //This is the same when we reference to component thanks to 'component' property. In my opinion it's bug the newly version of ui-router module.
    //States which define routing Angular app.
    var states = [{
        name: 'main',
        url: '/',
        template: '<main-page is-logged="Client.isLogged" list-creator="Client.listCreator" show-creator="Client.showCreator"' +
                  'change-visibility-creator="Client.showCreator = $event.showCreator" length-of-array-id-films="Client.lengthOfArray"' +
                  'is-admin = "Client.isAdmin" is-moderator="Client.isModerator" content-creator="Client.contentCreator"' +
                  'open-modal="Client.openModal"></main-page>'
    }, {
        name: 'editUserAccount',
        url: '/editUserAccount',
        template: '<edit-user-account></edit-user-account>'
    }, {
        name: 'userRegister',
        url: '/userRegister',
        template: '<user-register></user-register>'
    }, {
        name: 'userLogin',
        url: '/userLogin',
        template: '<user-login on-view-change="Client.isLogged = $event.isLogged"></user-login>'
    }, {
        name: 'listRanking',
        url:'/listRanking',
        template: '<list-ranking></list-ranking>',
    }, {
        name: 'showDetailsFilm',
        url:'/showDetailsFilm/:filmId',
        template: '<show-details-film is-logged="Client.isLogged" list-creator="Client.listCreator" show-creator="Client.showCreator"' +
                  'change-visibility-creator="Client.showCreator = $event.showCreator" length-of-array-id-films="Client.lengthOfArray"></show-details-film>'
    }, {
        name: 'retriveUserAccount',
        url: '/retriveUserAccount',
        template: "<retrive-user-account></retrive-user-account>"
    }, {
        name: 'favoriteUserList',
        url: '/favoriteUserList',
        template: '<favorite-user-list is-logged="Client.isLogged"></favorite-user-list>'
    },{
        name: 'ownUserList',
        url: '/ownUserList',
        template: '<own-user-list is-logged="Client.isLogged" show-creator="Client.showCreator"' +
                  'change-visibility-creator="Client.showCreator = $event.showCreator"' +
                  'content-creator="Client.contentCreator" list-creator="Client.listCreator"' +
                  'name-list="Client.nameList" edit-list="Client.editList"' +
                  'show-buttons="Client.showButtons" show-spinner="Client.showSpinner"' +
                  'id-list="Client.idList"></own-user-list>'
    }, {
        name: 'adminPanel',
        url: '/adminPanel',
        template: '<admin-panel is-admin="Client.isAdmin"></admin-panel>'
    }, {
        name: 'moderatorPanel',
        url: '/moderatorPanel',
        template: '<moderator-panel is-moderator = "Client.isModerator"></moderator-panel>'
    },{
        name:'resultSearchBar',
        url: '/resultSearchBar/:result',
        template: '<result-search-bar is-logged="Client.isLogged" list-creator="Client.listCreator" show-creator="Client.showCreator"' +
                  'change-visibility-creator="Client.showCreator = $event.showCreator" length-of-array-id-films="Client.lengthOfArray"></result-search-bar>'
    }
    ]

    states.forEach(function (state) {
        $stateProvider.state(state);
    });

    $urlRouterProvider.when('', '/')
        .when('main', '/')
        .when('editUserAccount', '/editUserAccount')
        .when('userRegister', '/userRegister')
        .when('userLogin', '/userLogin')
        .when('listRanking', '/listRanking')
        .when('showDetailsFilm', '/showDetailsFilm')
        .when('favoriteUserList', '/main')
        .when('retriveUserAccount', '/retriveUserAccount')
        .when('adminPanel', '/adminPanel')
        .when('moderatorPanel', '/moderatorPanel')
        .when('resultSearchBar', '/resultSearchBar/:result')
        .otherwise('/');
    //This provider add header of 'Authorization' which will be able in all required http requests.
    $httpProvider.interceptors.push('authInterceptorService');
})
.controller('MainLayoutController', function (localStorageService) {

    this.isLogged = localStorageService.get('isLogged');
    this.isAdmin =  localStorageService.get('isAdmin');
    this.isModerator = localStorageService.get('isModerator');

    this.contentCreator = false;
    this.showCreator = "none";
    this.listCreator = [];
    this.idList = 0;
    this.nameList = ""
    this.openModal = false;
    this.editList = false;
    this.showButtons = true;
    this.showSpinner = false;
    
})

.run(function () {
});
