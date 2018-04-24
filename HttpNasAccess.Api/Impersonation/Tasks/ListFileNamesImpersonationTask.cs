using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class ListFileNamesImpersonationTask : IImpersonationTask<IEnumerable<string>>
    {
        private readonly string _nasPath;

        public ListFileNamesImpersonationTask(string nasPath)
        {
            _nasPath = nasPath;
        }

        public IEnumerable<string> Execute()
        {
            var dirInfo = new DirectoryInfo(_nasPath);
            return dirInfo.EnumerateFiles().Select(fileInfo => fileInfo.Name);
        }
    }
}