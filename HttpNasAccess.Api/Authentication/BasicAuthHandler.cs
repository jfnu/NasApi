using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;

namespace HttpNasAccess.Api.Authentication
{
    public class BasicAuthHandler : AuthenticationHandler<BasicAuthOptions>
    {
        private readonly string _challenge;
        public BasicAuthHandler(BasicAuthOptions options)
        {
            _challenge = $"Basic realm={options.Realm}";
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var authValue = Request.Headers.Get("Authorization");
            if (string.IsNullOrEmpty(authValue) || !authValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                return null;

            var token = authValue.Replace("Basic", string.Empty).Trim();
            var claims = await GetCredentialFromToken(token, Options.CredentialValidationFunc);
            if (claims == null)
                return null;
            else
            {
                var id = new ClaimsIdentity(claims,Options.AuthenticationType);
                return new AuthenticationTicket(id, new AuthenticationProperties());
            }
        }

        protected override Task ApplyResponseChallengeAsync()
        {
           if (Response.StatusCode == 401)
            {
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
                if(challenge != null)
                    Response.Headers.AppendValues("WWW-Authenticate",_challenge);
            }
            return Task.FromResult<object>(null);
        }

        private async Task<IEnumerable<Claim>> GetCredentialFromToken(string token, BasicAuthMiddleware.CredentialValidationFunc credentialValidationFunc)
        {
            string readableToken;
            try
            {
                readableToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            }
            catch (Exception)
            {
                return null;
            }
            var pair = readableToken.Split(':');
            if (pair.Length != 2) return null;
            var userId = pair[0];
            var userPassword = pair[1];

            return await credentialValidationFunc(userId, userPassword);
        }
    }
}