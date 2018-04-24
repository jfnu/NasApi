using System;
using System.IO;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class TestAccessImpersonationTask : IImpersonationTask<bool>
    {
        private readonly string _path;

        public TestAccessImpersonationTask(string path)
        {
            _path = path;
        }

        public bool Execute()
        {
            var fileName = Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty) + ".txt";
            var target = Path.Combine(_path, fileName);
            var fileInfo = new FileInfo(target);
            if (fileInfo.Exists) fileInfo.Delete();
            using (var writer = new StreamWriter(target))
            {
                writer.Write("test");
                writer.Close();
            }
            fileInfo = new FileInfo(target);
            if (fileInfo.Exists) fileInfo.Delete();
            return true;
        }
    }
}