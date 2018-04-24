using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using HttpNasAccess.Api.Credential;
using HttpNasAccess.Api.Extractor;
using HttpNasAccess.Api.Impersonation;
using HttpNasAccess.Api.Impersonation.Tasks;

namespace HttpNasAccess.Api.Controllers
{
    
    [RoutePrefix("nas")]
    [Authorize]
    
    public class NasController : ApiController
    {
        private readonly string StagingFolder = WebConfigurationManager.AppSettings["IISStagingPath"];
        private readonly string Domain = WebConfigurationManager.AppSettings["Domain"] ?? "ms";

        private readonly IRequestCredential _credential;
        private readonly INasRequestExtractor _requestExtractor;
        private readonly IImpersonator _impersonator;

        public NasController()
        {
            _credential = new HttpRequestCredential();
            _impersonator = new Impersonator(new WindowsIdentityImpersonation());
            _requestExtractor = new NasRequestExtractor()
            {
                StagingFolder = StagingFolder
            };
        }

        [HttpPost]
        [Route("listfilenames")]
        public async Task<HttpResponseMessage> ListFileNames()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);

            var nonFileData = _requestExtractor.NonFileData.GetValues(0);
            //Get NAS Path
            var nasPath = string.Empty;
            if (nonFileData != null) nasPath = nonFileData[0];
            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                var impersonationTask = new ListFileNamesImpersonationTask(nasPath);
                var listFiles = _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
                return Helper.CreateResponseMessage(HttpStatusCode.OK, listFiles);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }


        [HttpPost]
        [Route("listall")]
        public async Task<HttpResponseMessage> ListAll()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);

            var nonFileData = _requestExtractor.NonFileData.GetValues(0);
            //Get NAS Path
            var nasPath = string.Empty;
            if (nonFileData != null) nasPath = nonFileData[0];
            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                var impersonationTask = new ListAllImpersonationTask(nasPath);
                var listFiles = _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
                return Helper.CreateResponseMessage(HttpStatusCode.OK, listFiles);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }

        [HttpPost]
        [Route("getinfo")]
        public async Task<HttpResponseMessage> GetInfo()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);

            var nonFileData = _requestExtractor.NonFileData.GetValues(0);
            //Get NAS Path
            var nasPath = string.Empty;
            if (nonFileData != null) nasPath = nonFileData[0];
            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                var impersonationTask = new ListInfoImpersonationTask(nasPath);
                var listFiles = _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
                return Helper.CreateResponseMessage(HttpStatusCode.OK, listFiles);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }
        [HttpPost]
        [Route("renamefile")]
        public async Task<HttpResponseMessage> RenameFile()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);
            var nonFileData = _requestExtractor.NonFileData;
            var oldFileFullPath = string.Empty;
            var newFileFullPath = string.Empty;
            if (nonFileData != null && nonFileData.Count>1)
            {
                oldFileFullPath = nonFileData.GetValues(0)[0];
                newFileFullPath = nonFileData.GetValues(1)[0];
            }
            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                var impersonationTask = new RenameFileImpersonationTask(oldFileFullPath, newFileFullPath);
                return !_impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain) 
                    ? Helper.CreateResponseMessage(HttpStatusCode.BadRequest, "Source file does not exists.") 
                    : Helper.CreateResponseMessage(HttpStatusCode.OK, "Rename operation completed successfully");
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }

        [HttpPost]
        [Route("copytoanothernasdrive")]
        public async Task<HttpResponseMessage> CopyToAnotherNasDrive()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);


            var nonFileData = _requestExtractor.NonFileData;
            //Get NAS Path
            var targetCopyFolder = string.Empty;
            List<string> itemsToCopy = new List<string>();

            if (nonFileData != null)
            {
                targetCopyFolder = nonFileData.GetValues(0)[0];
                if(nonFileData.Count>1)
                    itemsToCopy = nonFileData.GetValues(1).ToList();
            }
            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                var impersonationTask = new CopyToAnotherNasImpersonationTask(itemsToCopy,targetCopyFolder);
                _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
                return Helper.CreateResponseMessage(HttpStatusCode.OK,"Copy operation completed successfully");
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }


        [HttpPost]
        [Route("Delete")]
        public async Task<HttpResponseMessage> Delete()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);

            var data = _requestExtractor.NonFileData.GetValues(0);

            var fullFilePath = string.Empty;
            if (data != null) fullFilePath = data[0];

            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                 IImpersonationTask<bool> impersonationTask = new DeleteFileImpersonationTask(fullFilePath);
                _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
                return Helper.CreateResponseMessage(HttpStatusCode.OK, "File successfully deleted from NAS");
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<HttpResponseMessage> Upload()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);

            var nonFileData = _requestExtractor.NonFileData.GetValues(0);
            var fileData = _requestExtractor.FileData;

            //Get NAS Path
            var nasPath = string.Empty;
            if (nonFileData != null) nasPath = nonFileData[0];

            try
            {
                var nasIdentity = _credential.GetCurrentCredential(Request);
                IImpersonationTask<bool> impersonationTask = new UploadImpersonationTask(fileData, nasPath);
                _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }
        [HttpPost]
        [Route("download")]
        public async Task<HttpResponseMessage> Download()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(
                    _requestExtractor.Response.StatusCode,_requestExtractor.Response.Message);

            var nonFileData = _requestExtractor.NonFileData.GetValues(0);

            var fullFilePath = string.Empty;
            if (nonFileData != null) fullFilePath = nonFileData[0];
            
            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                var fileName = Path.GetFileName(fullFilePath);
                var nasPath = fullFilePath.Replace(fileName, string.Empty);
                IImpersonationTask<bool> impersonationTask = new TestAccessImpersonationTask(nasPath);
                _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }

            try
            {
                IImpersonationTask<HttpResponseMessage> impersonationTask = new DownloadImpersonationTask(fullFilePath);
                return _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.InternalServerError,
                  $"Unexpected error in downloading the file.-{fullFilePath}-{ex.Message}");
            }
        }

        [HttpPost]
        [Route("downloadmultiple")]
        public async Task<HttpResponseMessage> DownloadMultipleinZip()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);

            var nonFileData = _requestExtractor.NonFileData.GetValues(0);

            var fullFilePath = string.Empty;
            if (nonFileData != null) fullFilePath = nonFileData[0];

            var nasIdentity = _credential.GetCurrentCredential(Request);
            var nasPath = string.Empty;
            try
            {
                var fileName = Path.GetFileName(fullFilePath);
                nasPath = fullFilePath.Replace(fileName, string.Empty);
                IImpersonationTask<bool> impersonationTask = new TestAccessImpersonationTask(nasPath);
                _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }

            try
            {
                IImpersonationTask<HttpResponseMessage> impersonationTask = new DownloadMultipleImpersonationTask(nonFileData, nasPath);
                return _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.InternalServerError,
                  $"Unexpected error in downloading the file.-{nasPath}-{ex.Message}");
            }

        }


        #region Folder
        [HttpPost]
        [Route("folder/create")]
        public async Task<HttpResponseMessage> CreateFolder()
        {
            if (!await _requestExtractor.Extract(Request))
                return Helper.CreateResponseMessage(_requestExtractor.Response.StatusCode,
                    _requestExtractor.Response.Message);

            var data = _requestExtractor.NonFileData.GetValues(0);

            var fullFilePath = string.Empty;
            if (data != null) fullFilePath = data[0];

            var nasIdentity = _credential.GetCurrentCredential(Request);
            try
            {
                IImpersonationTask<bool> impersonationTask = new CreateFolderImpersonationTask(fullFilePath);
                _impersonator.ExecuteTask(impersonationTask, nasIdentity.UserId, nasIdentity.UserPassword, Domain);
                return Helper.CreateResponseMessage(HttpStatusCode.OK, "Folder successfully created on NAS");
            }
            catch (Exception ex)
            {
                return Helper.CreateResponseMessage(HttpStatusCode.Forbidden,
                    $"Error:{ex.Message}");
            }
        }
        #endregion


    }
}
