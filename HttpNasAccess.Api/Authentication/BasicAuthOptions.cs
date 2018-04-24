using Microsoft.Owin.Security;

namespace HttpNasAccess.Api.Authentication
{
    public class BasicAuthOptions : AuthenticationOptions
   {
        public string Realm { get; private set; }
        public BasicAuthMiddleware.CredentialValidationFunc CredentialValidationFunc { get; private set; }

        public BasicAuthOptions(string realm, BasicAuthMiddleware.CredentialValidationFunc validationFunc) : base("Basic")
        {
            Realm = realm;
            CredentialValidationFunc = validationFunc;
        }
   }
}
