using Owin;

namespace HttpNasAccess.Api.Authentication
{
    public static class BasicAuthMiddlewareExtensions
    {
        public static IAppBuilder UseBasicAuth(this IAppBuilder app, string realm,
            BasicAuthMiddleware.CredentialValidationFunc credentialValidationFunc)
        {
            var options = new BasicAuthOptions(realm,credentialValidationFunc);
            return app.UseBasicAuth(options);
        }

        public static IAppBuilder UseBasicAuth(this IAppBuilder app, BasicAuthOptions options)
        {
            return app.Use<BasicAuthMiddleware>(options);
        }
    }
}