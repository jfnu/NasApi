using HttpNasAccess.Api.Impersonation.Tasks;

namespace HttpNasAccess.Api.Impersonation
{
    public class Impersonator : IImpersonator
    {
        private readonly IImpersonation _impersonation;

        public Impersonator(IImpersonation impersonation)
        {
            _impersonation = impersonation;
        }

        public T ExecuteTask<T>(
            IImpersonationTask<T> impersonationTask,
            string userId, string userPassword, string domain)
        {
            using (var context = _impersonation.Impersonate(userId,userPassword,domain))
            {
                return impersonationTask.Execute();
            }
        }
       
    }
}