using API.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Service;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(API.Startup))]
namespace API
{
    public class Startup
    {
        private IAuthServices _authService;
        private IListServices _listService;
        private IFilmServices _filmServices;

        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            // initialize cache
            _authService = new AuthServices();
            _listService = new ListServices();
            _filmServices = new FilmServices();

            _filmServices.InitializeFilmsCache();
            _authService.InitializeUsersCache();
            _listService.InitializeListsCache();
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

    }
}