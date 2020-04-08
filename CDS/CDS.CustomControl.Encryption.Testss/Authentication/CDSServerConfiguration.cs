using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System;

namespace CDS.CustomControl.Encryption.Test.Authentication
{
    class CDSServerConfiguration : ServerConfiguration
    {
        public Uri DiscoveryUri;
        public Uri OrganizationUri;
        public AuthenticationProviderType EndpointType;
        public OrganizationDetail[] organizations;
        internal SecurityTokenResponse OrganizationTokenResponse;
        internal IServiceManagement<IOrganizationService> OrganizationServiceManagement;

        public CDSServerConfiguration()
        { }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            CDSServerConfiguration c = (CDSServerConfiguration)obj;

            if (!ServerAddress.Equals(c.ServerAddress, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (!OrganizationName.Equals(c.OrganizationName, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (EndpointType != c.EndpointType)
                return false;
            if (null != Credentials && null != c.Credentials)
            {
                if (EndpointType == AuthenticationProviderType.ActiveDirectory)
                {
                    if (!Credentials.Windows.ClientCredential.Domain.Equals(
                        c.Credentials.Windows.ClientCredential.Domain, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                    if (!Credentials.Windows.ClientCredential.UserName.Equals(
                        c.Credentials.Windows.ClientCredential.UserName, StringComparison.InvariantCultureIgnoreCase))
                        return false;
                }
                else
                {
                    if (!Credentials.UserName.UserName.Equals(c.Credentials.UserName.UserName,
                        StringComparison.InvariantCultureIgnoreCase))
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            int returnHashCode = this.ServerAddress.GetHashCode()
                ^ OrganizationName.GetHashCode()
                ^ EndpointType.GetHashCode();
            if (null != Credentials)
            {
                if (EndpointType == AuthenticationProviderType.ActiveDirectory)
                    returnHashCode = returnHashCode
                        ^ Credentials.Windows.ClientCredential.UserName.GetHashCode()
                        ^ Credentials.Windows.ClientCredential.Domain.GetHashCode();
                else
                    returnHashCode = returnHashCode
                        ^ Credentials.UserName.UserName.GetHashCode();
            }
            return returnHashCode;
        }
    }
}
