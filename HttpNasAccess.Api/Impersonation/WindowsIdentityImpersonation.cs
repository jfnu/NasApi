using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace HttpNasAccess.Api.Impersonation
{
    public class WindowsIdentityImpersonation : IImpersonation
    {

        [DllImport("advapi32.DLL", SetLastError = true)]
        private static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
int dwLogonProvider, ref IntPtr phToken);

        public WindowsImpersonationContext Impersonate(string userId, string userPassword, string domain)
        {
            var tempToken = new IntPtr();
            return Convert.ToBoolean(LogonUser(userId, domain, userPassword, 9, 0, ref tempToken)) ? 
                WindowsIdentity.Impersonate(tempToken) : null;
        }
    }
}