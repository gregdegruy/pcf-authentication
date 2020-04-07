using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace CDS.CustomControl.Encryption.Test.Authentication
{
    internal abstract class ServerConnection
    {
        public static SecureString ReadPassword()
        {
            SecureString ssPassword = new SecureString();

            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key == ConsoleKey.Backspace)
                {
                    if (ssPassword.Length != 0)
                    {
                        ssPassword.RemoveAt(ssPassword.Length - 1);
                        Console.Write("\b \b"); // erase last char
                    }
                }
                else if (info.KeyChar >= ' ') // no control chars
                {
                    ssPassword.AppendChar(info.KeyChar);
                    Console.Write("*");
                }
                info = Console.ReadKey(true);
            }

            Console.WriteLine();
            Console.WriteLine();

            ssPassword.MakeReadOnly();

            return ssPassword;
        }

        public static String ConvertToUnsecureString(SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ConvertToSecureString(string password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();
            foreach (char c in password)
                securePassword.AppendChar(c);
            securePassword.MakeReadOnly();
            return securePassword;
        }

        internal sealed class Credential
        {
            private SecureString _userName;
            private SecureString _password;

            internal Credential(CREDENTIAL_STRUCT cred)
            {
                _userName = ConvertToSecureString(cred.userName);
                int size = (int)cred.credentialBlobSize;
                if (size != 0)
                {
                    byte[] bpassword = new byte[size];
                    Marshal.Copy(cred.credentialBlob, bpassword, 0, size);
                    _password = ConvertToSecureString(Encoding.Unicode.GetString(bpassword));
                }
                else
                {
                    _password = ConvertToSecureString(String.Empty);
                }
            }

            public Credential(string userName, string password)
            {
                if (String.IsNullOrWhiteSpace(userName))
                    throw new ArgumentNullException("userName");
                if (String.IsNullOrWhiteSpace(password))
                    throw new ArgumentNullException("password");

                _userName = ConvertToSecureString(userName);
                _password = ConvertToSecureString(password);
            }

            public string UserName
            {
                get { return ConvertToUnsecureString(_userName); }
            }

            public string Password
            {
                get { return ConvertToUnsecureString(_password); }
            }

            /// <summary>
            /// This converts a SecureString password to plain text
            /// </summary>
            /// <param name="securePassword">SecureString password</param>
            /// <returns>plain text password</returns>
            private string ConvertToUnsecureString(SecureString secret)
            {
                if (secret == null)
                    return string.Empty;

                IntPtr unmanagedString = IntPtr.Zero;
                try
                {
                    unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secret);
                    return Marshal.PtrToStringUni(unmanagedString);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                }
            }

            /// <summary>
            /// This converts a string to SecureString
            /// </summary>
            /// <param name="password">plain text password</param>
            /// <returns>SecureString password</returns>
            private SecureString ConvertToSecureString(string secret)
            {
                if (string.IsNullOrEmpty(secret))
                    return null;

                SecureString securePassword = new SecureString();
                char[] passwordChars = secret.ToCharArray();
                foreach (char pwdChar in passwordChars)
                {
                    securePassword.AppendChar(pwdChar);
                }
                securePassword.MakeReadOnly();
                return securePassword;
            }


            /// <summary>
            /// This structure maps to the CREDENTIAL structure used by native code. We can use this to marshal our values.
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            internal struct CREDENTIAL_STRUCT
            {
                public UInt32 flags;
                public UInt32 type;
                public string targetName;
                public string comment;
                public System.Runtime.InteropServices.ComTypes.FILETIME lastWritten;
                public UInt32 credentialBlobSize;
                public IntPtr credentialBlob;
                public UInt32 persist;
                public UInt32 attributeCount;
                public IntPtr credAttribute;
                public string targetAlias;
                public string userName;
            }

        }

        internal static class CredentialManager
        {
            /// <summary>
            /// Target Name against which all credentials are stored on the disk.
            /// </summary>
            public const string TargetName = "Microsoft_CRMSDK:";
            /// <summary>
            /// Cache containing secrets in-memory (used to improve performance and avoid IO operations).
            /// </summary>
            private static Dictionary<string, Credential> credentialCache = new Dictionary<string, Credential>();

            public static Uri GetCredentialTarget(Uri target)
            {
                if (null == target)
                    throw new ArgumentNullException("target");
                return new Uri(target.GetLeftPart(UriPartial.Authority));
            }

            private enum CRED_TYPE : int
            {
                GENERIC = 1,
                DOMAIN_PASSWORD = 2,
                DOMAIN_CERTIFICATE = 3,
                DOMAIN_VISIBLE_PASSWORD = 4,
                MAXIMUM = 5
            }

            internal enum CRED_PERSIST : uint
            {
                SESSION = 1,
                LOCAL_MACHINE = 2,
                ENTERPRISE = 3
            }

            private static class NativeMethods
            {
                [DllImport("advapi32.dll", SetLastError = true,
                    EntryPoint = "CredReadW", CharSet = CharSet.Unicode)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool CredRead(string target, CRED_TYPE type, int reservedFlag,
                    [MarshalAs(UnmanagedType.CustomMarshaler,
                    MarshalTypeRef = typeof(CredentialMarshaler))] out Credential credential);

                [DllImport("Advapi32.dll", SetLastError = true,
                    EntryPoint = "CredWriteW", CharSet = CharSet.Unicode)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool CredWrite(ref Credential.CREDENTIAL_STRUCT credential, UInt32 flags);

                [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool CredFree(IntPtr cred);

                [DllImport("advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode)]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool CredDelete(string target, int type, int flags);
            }

            private sealed class CredentialMarshaler : ICustomMarshaler
            {
                private static CredentialMarshaler _instance;

                public void CleanUpManagedData(object ManagedObj)
                {
                    // Garbage collection, do nothing
                }

                public void CleanUpNativeData(IntPtr pNativeData)
                {
                    if (pNativeData == IntPtr.Zero)
                    {
                        return;
                    }
                    NativeMethods.CredFree(pNativeData);
                }

                public int GetNativeDataSize() { throw new NotImplementedException("The method or operation is not implemented."); }

                public IntPtr MarshalManagedToNative(object obj) { throw new NotImplementedException("The method or operation is not implemented."); }

                public object MarshalNativeToManaged(IntPtr pNativeData)
                {
                    if (pNativeData == IntPtr.Zero)
                    {
                        return null;
                    }
                    return new Credential((Credential.CREDENTIAL_STRUCT)Marshal.PtrToStructure(pNativeData, typeof(Credential.CREDENTIAL_STRUCT)));
                }


                public static ICustomMarshaler GetInstance(string cookie)
                {
                    if (null == _instance)
                        _instance = new CredentialMarshaler();
                    return _instance;
                }
            }

            public static Credential ReadCredentials(String target)
            {
                Credential cachedCredential;

                if (credentialCache.TryGetValue(TargetName + target, out cachedCredential))
                {
                    return cachedCredential;
                }

                Credential credential;
                bool bSuccess = NativeMethods.CredRead(TargetName + target, CRED_TYPE.GENERIC, 0, out credential);
                if (!bSuccess)
                {
                    return null;
                }

                credentialCache[TargetName + target.ToString()] = credential;
                return credential;
            }

            public static Credential ReadWindowsCredential(Uri target)
            {
                Credential credential;
                bool bSuccess = NativeMethods.CredRead(target.Host, CRED_TYPE.DOMAIN_PASSWORD, 0, out credential);
                if (!bSuccess)
                {
                    throw new InvalidOperationException("Unable to read windows credentials for Uri {0}. ErrorCode {1}",
                        new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error()));
                }
                return credential;
            }

            public static void WriteCredentials(String target, Credential userCredentials, bool allowPhysicalStore)
            {
                if (String.IsNullOrWhiteSpace(target))
                {
                    throw new ArgumentNullException("target");
                }

                if (null == userCredentials)
                {
                    throw new ArgumentNullException("userCredentials");
                }

                credentialCache[TargetName + target] = userCredentials;

                string passwordToStore = allowPhysicalStore ? userCredentials.Password : string.Empty;
                Credential.CREDENTIAL_STRUCT credential = new Credential.CREDENTIAL_STRUCT();
                try
                {
                    credential.targetName = TargetName + target;
                    credential.type = (UInt32)CRED_TYPE.GENERIC;
                    credential.userName = userCredentials.UserName;
                    credential.attributeCount = 0;
                    credential.persist = (UInt32)CRED_PERSIST.LOCAL_MACHINE;
                    byte[] bpassword = Encoding.Unicode.GetBytes(passwordToStore);
                    credential.credentialBlobSize = (UInt32)bpassword.Length;
                    credential.credentialBlob = Marshal.AllocCoTaskMem(bpassword.Length);
                    Marshal.Copy(bpassword, 0, credential.credentialBlob, bpassword.Length);
                    if (!NativeMethods.CredWrite(ref credential, 0))
                    {
                        throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                finally
                {
                    if (IntPtr.Zero != credential.credentialBlob)
                        Marshal.FreeCoTaskMem(credential.credentialBlob);
                }
            }

            public static void DeleteCredentials(String target, bool softDelete)
            {
                if (String.IsNullOrWhiteSpace(target))
                {
                    throw new ArgumentNullException("target");
                }

                if (softDelete)
                {
                    // Removes only the password
                    try
                    {
                        Credential tempCredential = ReadCredentials(target);
                        WriteCredentials(target, new Credential(tempCredential.UserName, String.Empty), true);
                    }
                    catch (Exception) { }
                }
                else
                {
                    // Removes the entry completely
                    NativeMethods.CredDelete(TargetName + target, (int)CRED_TYPE.GENERIC, 0);
                    credentialCache.Remove(TargetName + target);
                }
            }
        }

        /// <summary>
        /// Wrapper class for DiscoveryServiceProxy to support auto refresh security token.
        /// </summary>
        internal sealed class ManagedTokenDiscoveryServiceProxy : DiscoveryServiceProxy
        {
            private AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService> _proxyManager;

            public ManagedTokenDiscoveryServiceProxy(Uri serviceUri, ClientCredentials userCredentials)
                : base(serviceUri, null, userCredentials, null)
            {
                this._proxyManager = new AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService>(this);
            }

            public ManagedTokenDiscoveryServiceProxy(IServiceManagement<IDiscoveryService> serviceManagement,
                SecurityTokenResponse securityTokenRes)
                : base(serviceManagement, securityTokenRes)
            {
                this._proxyManager = new AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService>(this);
            }

            public ManagedTokenDiscoveryServiceProxy(IServiceManagement<IDiscoveryService> serviceManagement,
               ClientCredentials userCredentials)
                : base(serviceManagement, userCredentials)
            {
                this._proxyManager = new AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService>(this);
            }

            protected override void AuthenticateCore()
            {
                this._proxyManager.PrepareCredentials();
                base.AuthenticateCore();
            }

            protected override void ValidateAuthentication()
            {
                this._proxyManager.RenewTokenIfRequired();
                base.ValidateAuthentication();
            }
        }

        /// <summary>
        /// Wrapper class for OrganizationServiceProxy to support auto refresh security token
        /// </summary>
        internal sealed class ManagedTokenOrganizationServiceProxy : OrganizationServiceProxy
        {
            private AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService> _proxyManager;

            public ManagedTokenOrganizationServiceProxy(Uri serviceUri, ClientCredentials userCredentials)
                : base(serviceUri, null, userCredentials, null)
            {
                this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
            }

            public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement,
                SecurityTokenResponse securityTokenRes)
                : base(serviceManagement, securityTokenRes)
            {
                this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
            }

            public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement,
                ClientCredentials userCredentials)
                : base(serviceManagement, userCredentials)
            {
                this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
            }

            protected override void AuthenticateCore()
            {
                this._proxyManager.PrepareCredentials();
                base.AuthenticateCore();
            }

            protected override void ValidateAuthentication()
            {
                this._proxyManager.RenewTokenIfRequired();
                base.ValidateAuthentication();
            }
        }

        /// <summary>
        /// Class that wraps acquiring the security token for a service
        /// </summary>
        public sealed class AutoRefreshSecurityToken<TProxy, TService>
            where TProxy : ServiceProxy<TService>
            where TService : class
        {
            private TProxy _proxy;

            /// <param name="proxy">Proxy that will be used to authenticate the user</param>
            public AutoRefreshSecurityToken(TProxy proxy)
            {
                if (null == proxy)
                {
                    throw new ArgumentNullException("proxy");
                }

                this._proxy = proxy;
            }

            /// <summary>
            /// Prepares authentication before authenticated
            /// </summary>
            public void PrepareCredentials()
            {
                if (null == this._proxy.ClientCredentials)
                {
                    return;
                }

                switch (this._proxy.ServiceConfiguration.AuthenticationType)
                {
                    case AuthenticationProviderType.ActiveDirectory:
                        this._proxy.ClientCredentials.UserName.UserName = null;
                        this._proxy.ClientCredentials.UserName.Password = null;
                        break;
                    case AuthenticationProviderType.Federation:
                        this._proxy.ClientCredentials.Windows.ClientCredential = null;
                        break;
                    default:
                        return;
                }
            }


            /// <summary>
            /// Renews the token (if it is near expiration or has expired)
            /// </summary>
            public void RenewTokenIfRequired()
            {
                //if (null != this._proxy.SecurityTokenResponse &&
                //    DateTime.UtcNow.AddMinutes(15) >= this._proxy.SecurityTokenResponse.Response.Lifetime.Expires)
                //{
                //    try
                //    {
                //        this._proxy.Authenticate();
                //    }
                //    catch (CommunicationException)
                //    {
                //        if (null == this._proxy.SecurityTokenResponse ||
                //            DateTime.UtcNow >= this._proxy.SecurityTokenResponse.Response.Lifetime.Expires)
                //        {
                //            throw;
                //        }
                //    }
                //}
            }
        }
    }
}