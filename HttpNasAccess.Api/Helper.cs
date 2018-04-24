using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HttpNasAccess.Api
{
    public class Helper
    {
        public static HttpResponseMessage CreateResponseMessage(HttpStatusCode statusCode, string message)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(message)
            };
        }

        public static HttpResponseMessage CreateResponseMessage<T>(HttpStatusCode statusCode, T message)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new ObjectContent<T>(message, new JsonMediaTypeFormatter())
            };
        }
    }
}
