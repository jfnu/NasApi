using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpNasAccess.Api.Model;

namespace HttpNasAccess.Api.Extractor
{
    public class NasRequestExtractor : INasRequestExtractor
    {
        public string StagingFolder { get; set; }
        public ResponseMessage Response { get; set; }
        public Collection<MultipartFileData> FileData { get; private set; }
        public NameValueCollection NonFileData { get; private set; }
      
        public async Task<bool> Extract(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent("form-data"))
            {
                Response = CreateResponse(HttpStatusCode.UnsupportedMediaType,
                    "Need to post request in multipart form data format.");
                return false;
            }

            MultipartFormDataStreamProvider bodyParts;
            try
            {
                var streamProvider = new MultipartFormDataStreamProvider(StagingFolder);
                bodyParts = await request.Content.ReadAsMultipartAsync(streamProvider);
            }
            catch (Exception ex)
            {
                Response = CreateResponse(HttpStatusCode.InternalServerError,
                    ex.Message);
                return false;
            }

            NonFileData = bodyParts.FormData;
            if (NonFileData.Keys.Count == 0)
            {
                Response = CreateResponse(HttpStatusCode.BadRequest,
                    "Missing Path in the request.");
                return false;
            }
            FileData = bodyParts.FileData;
            return true;

        }

        private ResponseMessage CreateResponse(HttpStatusCode statusCode, string message)
        {
            return new ResponseMessage
            {
                Message = message,
                StatusCode = statusCode
            };
        }
    }
}