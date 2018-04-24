using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using HttpNasAccess.Api.Model;

namespace HttpNasAccess.Api.Credential
{
    public class HttpRequestCredential : IRequestCredential
    {
        public NasIdentity GetCurrentCredential(HttpRequestMessage request)
        {
            var principal = request.GetRequestContext().Principal as ClaimsPrincipal;
            var claims = principal.Claims.ToList();
            return new NasIdentity
            {
                UserId = claims[0].Value,
                UserPassword = claims[1].Value
            };
        }
    }
}