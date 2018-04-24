using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using HttpNasAccess.Api.Authentication;
using HttpNasAccess.Api.RequireTls;
using Owin;

namespace HttpNasAccess.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var httpConfiguration = new HttpConfiguration();
            var routes = RouteTable.Routes;
            RouteConfig.RegisterRoutes(routes);
            WebApiConfig.Register(httpConfiguration);
            appBuilder.UseRequireTls();
            appBuilder.UseBasicAuth("Basic", ValidateUser);
            appBuilder.UseWebApi(httpConfiguration);
        }

        private Task<IEnumerable<Claim>> ValidateUser(string userId, string userPassword)
        {
            using (var pc = new PrincipalContext(ContextType.Domain))
            {
                if (pc.ValidateCredentials(userId, userPassword, ContextOptions.Negotiate))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId),
                        new Claim(ClaimTypes.UserData, userPassword)
                    };
                    return Task.FromResult<IEnumerable<Claim>>(claims);
                }
            }
            return Task.FromResult<IEnumerable<Claim>>(null);
        }
    }
}
