using System.IO;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class DeleteFileImpersonationTask : IImpersonationTask<bool>
    {
        private readonly string _filePathInNas;

        public DeleteFileImpersonationTask(string filePathInNas)
        {
            _filePathInNas = filePathInNas;
        }

        public bool Execute()
        {
            var fileInfo = new FileInfo(_filePathInNas);
            if (fileInfo.Exists) fileInfo.Delete();
            return true;
        }
    }
}