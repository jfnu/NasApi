using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class UploadImpersonationTask : IImpersonationTask<bool>
    {
        private readonly ICollection<MultipartFileData> _fileData;
        private readonly string _nasPath;

        public UploadImpersonationTask(ICollection<MultipartFileData> fileData, string nasPath)
        {
            _fileData = fileData;
            _nasPath = nasPath;
        }

        public bool Execute()
        {
            foreach (var file in _fileData)
            {
                var fileName = SanitizeFileName(file.Headers.ContentDisposition.FileName);
                var target = Path.Combine(_nasPath, fileName);
                var fileInfo = new FileInfo(target);
                if (fileInfo.Exists) fileInfo.Delete();
                File.Move(file.LocalFileName, target);
            }
            return true;
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