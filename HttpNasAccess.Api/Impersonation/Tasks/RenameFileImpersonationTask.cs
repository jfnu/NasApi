using System.IO;

namespace HttpNasAccess.Api.Impersonation.Tasks
{
    public class RenameFileImpersonationTask : IImpersonationTask<bool>
    {
        private readonly string _oldFilePathInNas;
        private readonly string _newFilePathInNas;

        public RenameFileImpersonationTask(string oldFilePathInNas, string newFilePathInNas)
        {
            _oldFilePathInNas = oldFilePathInNas;
            _newFilePathInNas = newFilePathInNas;
        }

        public bool Execute()
        {
            var oldFileInfo = new FileInfo(_oldFilePathInNas);

            if (!oldFileInfo.Exists) return false;

            oldFileInfo.CopyTo(_newFilePathInNas, true);
            oldFileInfo.Delete();
            return true;
        }
    }
}