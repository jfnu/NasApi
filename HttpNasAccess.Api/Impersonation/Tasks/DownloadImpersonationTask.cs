using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class DownloadImpersonationTask : IImpersonationTask<HttpResponseMessage>
    {
        private readonly string _downloadFilePath;

        public DownloadImpersonationTask(string downloadFilePath)
        {
            _downloadFilePath = downloadFilePath;
        }

        public HttpResponseMessage Execute()
        {
            var fileName = Path.GetFileName(_downloadFilePath);

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(_downloadFilePath, FileMode.Open, FileAccess.Read);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            return response;
        }
    }
}