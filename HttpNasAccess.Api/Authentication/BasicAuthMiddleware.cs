using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;

namespace HttpNasAccess.Api.Authentication
{
    public class BasicAuthMiddleware :AuthenticationMiddleware<BasicAuthOptions>
    {
        public delegate Task<IEnumerable<Claim>> CredentialValidationFunc(string userId, string userPassword);
        public BasicAuthMiddleware(OwinMiddleware next, BasicAuthOptions options) : base(next, options) {}

        protected override AuthenticationHandler<BasicAuthOptions> CreateHandler()
        {
            return new BasicAuthHandler(Options);
        }
    }
}