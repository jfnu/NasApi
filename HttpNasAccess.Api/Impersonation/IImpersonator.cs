using HttpNasAccess.Api.Impersonation.Tasks;

namespace HttpNasAccess.Api.Impersonation
{
    public interface IImpersonator
    {
        T ExecuteTask<T>(
            IImpersonationTask<T> impersonationTask, 
            string userId, string userPassword, string domain);
    }
}