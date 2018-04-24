using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace HttpNasAccess.Api.RequireTls
{
    public static class RequireTlsMiddlewareExtension
    {
        public static IAppBuilder UseRequireTls(this IAppBuilder app)
        {
            return app.Use<RequireTlsMiddleware>();
        }
    }
}
