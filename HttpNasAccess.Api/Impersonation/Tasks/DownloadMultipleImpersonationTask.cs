using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class DownloadMultipleImpersonationTask : IImpersonationTask<HttpResponseMessage>
    {
        private readonly string[] _fileData;
        private readonly string _nasPath;

        public DownloadMultipleImpersonationTask(string[] fileData, string nasPath)
        {
            _fileData = fileData;
            _nasPath = nasPath;
        }

        public HttpResponseMessage Execute()
        {

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var tempFolderName = Guid.NewGuid().ToString().ToLower().Replace("-", "");
            var tempFolder = $@"{_nasPath}\{tempFolderName}";
            var zipFolder = $@"{_nasPath}\AzureNasTemp";

            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);
            if (!Directory.Exists(zipFolder)) Directory.CreateDirectory(zipFolder);

            var zipFullFileName = $@"{zipFolder}\{tempFolderName}.zip";

            var anyFileExists = false;

            for (int i = 0; i < _fileData.Length; i++)
            {
                var fullPathfileName = SanitizeFileName(_fileData[i]);
                var fileName = SanitizeFileName(Path.GetFileName(fullPathfileName));
                if (File.Exists(fullPathfileName))
                {
                    File.Copy(fullPathfileName, $@"{tempFolder}\{fileName}", true);
                    anyFileExists = true;
                }
            }

            if(anyFileExists) ZipFile.CreateFromDirectory(tempFolder, zipFullFileName);

            Thread.Sleep(3000);

            Directory.Delete(tempFolder,true);


            var stream = new FileStream(zipFullFileName, FileMode.Open);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Path.GetFileName(zipFullFileName)
            };

            return response;
        }
        private string SanitizeFileName(string fileName)
        {
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
            {
                fileName = fileName.Trim('"');
            }
            //if (fileName.Contains(@"/") || fileName.Contains(@"\"))
            //{
            //    fileName = Path.GetFileName(fileName);
            //}
            return fileName;
        }
    }
}