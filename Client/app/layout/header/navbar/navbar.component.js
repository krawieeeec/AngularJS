angular.module('navbar').component('navbar', {

    templateUrl: 'app/layout/header/navbar/navbar.template.html',
    controllerAs: 'Navbar',
    controller: function NavbarController (localStorageService, $window) {

      var ctrl = this;

      ctrl.toggleMoblileMenu = function(){
        var navigationToggle = angular.element( document.querySelector( '#navigation-toggle' ) );
        var navigation = angular.element( document.querySelector( '#navigation' ) );
        if (navigationToggle.hasClass('open-menu')){
          navigationToggle.removeClass('open-menu');
          navigation.toggle("slideUp");
        }else{
          navigationToggle.addClass('open-menu');
          navigation.toggle("slideUp");
        }
      }

    }

})
