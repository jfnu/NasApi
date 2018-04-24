using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class ListAllImpersonationTask : IImpersonationTask<IEnumerable<string>>
    {
        private readonly string _nasPath;

        public ListAllImpersonationTask(string nasPath)
        {
            _nasPath = nasPath;
        }

        public IEnumerable<string> Execute()
        {
            var dirInfo = new DirectoryInfo(_nasPath);
            var getAllFileNames = dirInfo
                .EnumerateFiles()
                .Select(fileInfo => fileInfo.Name);
            return getAllFileNames
                .Concat(
                    dirInfo.EnumerateDirectories().Select(info => info.Name));
        }
    }
}