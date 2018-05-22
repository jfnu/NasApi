using System;

using System.Runtime.ConstrainedExecution;

using System.Runtime.InteropServices;

using System.Security;

using System.Security.Principal;

using Microsoft.Win32.SafeHandles;

 

namespace HttpNasAccess.Api.Impersonation

{

    public class WindowsIdentityImpersonation : IImpersonation

    {

 

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]

        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,

       int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

 

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

        public static extern bool CloseHandle(IntPtr handle);

       

        public WindowsImpersonationContext Impersonate(string userId, string userPassword, string domain)

        {

            const int logon32ProviderDefault = 0;

            //This parameter causes LogonUser to create a primary token.

            const int logon32LogonInteractive = 2;

 

            SafeTokenHandle safeTokenHandle;

            var returnValue = Convert.ToBoolean(

                LogonUser(userId, domain, userPassword, logon32LogonInteractive, logon32ProviderDefault, out safeTokenHandle));

 

            if (!returnValue) return null;

 

            using (safeTokenHandle)

            {

                return WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle());

            }

               

        }

    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid

    {

        private SafeTokenHandle()

            : base(true)

        {

        }

 

        [DllImport("kernel32.dll")]

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]

        [SuppressUnmanagedCodeSecurity]

        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool CloseHandle(IntPtr handle);

 

        protected override bool ReleaseHandle()

        {

            return CloseHandle(handle);

        }

    }

 

 

 

}
