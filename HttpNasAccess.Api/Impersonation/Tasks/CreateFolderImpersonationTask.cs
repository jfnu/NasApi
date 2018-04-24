using System.IO;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class CreateFolderImpersonationTask : IImpersonationTask<bool>
    {
        private readonly string _folderPathInNas;

        public CreateFolderImpersonationTask(string folderPathInNas)
        {
            _folderPathInNas = folderPathInNas;
        }

        public bool Execute()
        {
            var directoryInfo = new DirectoryInfo(_folderPathInNas);
            if (!directoryInfo.Exists) directoryInfo.Create();
            return true;
        }
    }
}