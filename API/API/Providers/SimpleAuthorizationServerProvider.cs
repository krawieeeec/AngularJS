using Microsoft.Owin.Security.OAuth;
using Service;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IAuthServices _authService;

        public SimpleAuthorizationServerProvider()
        {
            _authService = new AuthServices();
        }
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var userId = await _authService.FindUser(context.UserName, context.Password);

            if (userId == "")
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            // claim for getting current user username
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            // registered user
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

            //this loop is where the roles are added as claims
            var userRoles = _authService.GetUserRolesByUserId(userId);
            foreach (var role in userRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            context.Validated(identity);
        }
    }
}