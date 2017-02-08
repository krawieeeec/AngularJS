//This service perform crud operations on users from server.

angular.module("moodies").factory("UsersService", function ($resource) {

    var _baseUrl = 'http://moodies.pl';

    let _recoverPassword = $resource(_baseUrl + '/api/accounts/forgotPassword');
    let _changeEmail = $resource(_baseUrl + '/api/accounts/changeEmail');
    let _editUserData = $resource(_baseUrl + 'api/accounts/changeUserDetails');
    let _obtainLoggedUserInfo = $resource(_baseUrl + '/api/accounts/userInfo');
    let _changePassword = $resource(_baseUrl + '/api/accounts/changePassword');
    let _addFavoriteList = $resource(_baseUrl + '/api/accounts/AddToFavourites');
    let _removeFromFavorite = $resource(_baseUrl + '/api/accounts/removeFavourite/:listId',{listId: '@listId'});
    let _rateFilm = $resource(_baseUrl + '/api/lists/rateFilm');
    let _getFavouritesLists = $resource(_baseUrl + '/api/accounts/:userName' + '/favourites', {userName: '@userName'});
    let _getOwnLists = $resource(_baseUrl + '/api/accounts/:userName' + '/created', {userName: '@userName'});
    let _getAllUsers = $resource(_baseUrl + '/api/accounts/UsersNames');
    let _deleteUser = $resource(_baseUrl + '/api/accounts/deleteUser/:userName', {userName: '@userName'});
    let _setRole = $resource(_baseUrl + '/api/accounts/AssignRolesToUser',{}, {
        'update': {
            method: 'PUT'
        }
    });


    return {

        RecoverPassword: _recoverPassword,
        ChangeEmail: _changeEmail,
        EditUserData: _editUserData,
        ObtainLoggedUserInfo: _obtainLoggedUserInfo,
        ChangePassword: _changePassword,
        AddFavoriteList: _addFavoriteList,
        RemoveFromFavorite: _removeFromFavorite,
        RateFilm: _rateFilm,
        GetFavouritesLists: _getFavouritesLists,
        GetOwnLists: _getOwnLists,
        GetAllUsers: _getAllUsers,
        DeleteUser: _deleteUser,
        SetRole: _setRole

    }
})
