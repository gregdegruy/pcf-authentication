using System;
using System.ServiceModel.Description;

namespace CDS.CustomControl.Encryption.Test.Authentication
{
    internal abstract class ServerConfiguration
    {
        public String ServerAddress;
        public String OrganizationName;        
        public Uri HomeRealmUri;
        private ClientCredentials _credentials;
        public ClientCredentials Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }
        // TODO must protect Username and password, can view in debugger
        public string Username
        {
            get { return _credentials.UserName.UserName; }
            set { _credentials.UserName.UserName = value; }
        }
        public string Password
        {
            get { return _credentials.UserName.Password; }
            set { _credentials.UserName.Password = value; }
        }
        public String UserPrincipalName;        
        internal Int16 AuthFailureCount = 0; 

        public ServerConfiguration()
        {
            _credentials = new ClientCredentials();
        }
    }
}
