using System.Collections.Generic;
using System.IO;
using System.Linq;
using HttpNasAccess.Api.Model;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class ListInfoImpersonationTask : IImpersonationTask<IEnumerable<FileDirectoryInfo>>
    {
        private readonly string _nasPath;

        public ListInfoImpersonationTask(string nasPath)
        {
            _nasPath = nasPath;
        }

        public IEnumerable<FileDirectoryInfo> Execute()
        {
            var dirInfo = new DirectoryInfo(_nasPath);
            var getAllFileNames = dirInfo
                .EnumerateFiles()
                .Select(fileInfo => new FileDirectoryInfo
                {
                    Name = fileInfo.Name,
                    Created = fileInfo.CreationTimeUtc,
                    Modified = fileInfo.LastWriteTimeUtc,
                    Size = fileInfo.Length
                });
            return getAllFileNames.Concat(
                dirInfo.EnumerateDirectories().Select(info => new FileDirectoryInfo
                {
                    Name = info.Name,
                    Created = info.CreationTimeUtc,
                    Modified = info.LastWriteTimeUtc,
                    Size = 0
                })
            );
        }
    }
}