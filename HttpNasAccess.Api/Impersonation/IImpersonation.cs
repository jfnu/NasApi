using System.Security.Principal;

namespace HttpNasAccess.Api.Impersonation
{
    public interface IImpersonation
    {
        WindowsImpersonationContext Impersonate(string userId, string userPassword, string domain);
    }
}
