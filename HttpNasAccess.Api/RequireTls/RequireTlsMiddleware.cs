using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace HttpNasAccess.Api.RequireTls
{
    public class RequireTlsMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public RequireTlsMiddleware(Func<IDictionary<string,object>,Task> next)
        {
            _next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);
            if (context.Request.Uri.Scheme != Uri.UriSchemeHttps)
            {
                context.Response.StatusCode = 403; //HttpStatusCode.Forbidden
                context.Response.ReasonPhrase = "SSL is required.";
                return;
            }
            await _next(environment);
        }
    }
}
