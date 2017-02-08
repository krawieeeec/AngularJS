angular.module('moodies')
.factory('StoreLoggedState', function () {
    
    var _storeLoggedState;
    
    return{
        get() {
            return _storeLoggedState;
        },
        set(isLogged){
            _storeLoggedState = isLogged;
        },
    }
})