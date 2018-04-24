using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class CopyToAnotherNasImpersonationTask : IImpersonationTask<bool>
    {
        private readonly List<string> _fileData;
        private readonly string _targetNasPath;

        public CopyToAnotherNasImpersonationTask(List<string> fileData, string targetNasPath)
        {
            _fileData = fileData;
            _targetNasPath = targetNasPath;
        }

        public bool Execute()
        {
            if (!Directory.Exists(_targetNasPath)) Directory.CreateDirectory(_targetNasPath);

            foreach (var item in _fileData)
            {
                var fullPath = SanitizeFileName(item);

                if (File.Exists(fullPath))
                {
                    var fileName = SanitizeFileName(Path.GetFileName(fullPath));
                    File.Copy(fullPath, $@"{_targetNasPath}\{fileName}", true);
                }
                else if (Directory.Exists(fullPath))
                {
                    CopyDirectory(fullPath, _targetNasPath);
                }
            }
          
            return true;
        }

        private void CopyDirectory(string source, string target)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(source, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, target));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, target), true);
        }
        private string SanitizeFileName(string fileName)
        {
            if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
            {
                fileName = fileName.Trim('"');
            }
            return fileName;
        }
    }
}