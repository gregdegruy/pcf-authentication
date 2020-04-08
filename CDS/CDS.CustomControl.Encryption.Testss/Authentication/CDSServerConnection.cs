using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;


namespace CDS.CustomControl.Encryption.Test.Authentication
{
    class CDSServerConnection : ServerConnection
    {
        enum Regions
        {
            UnitedStates,
            SouthAmerica,
            EuropeMiddleEastAfrica,
            AsiaPacific,
            Australia,
            Japan,
            India,
            UnitedKindgom
        };
        private readonly string[] dataCenters = {
            "crm",
            "crm2",
            "crm3",
            "crm4",
            "crm5",
            "crm6",
            "crm7",
            "crm8",
            "crm11"
        };
        private CDSServerConfiguration configuration;
        const string partialDiscoveryUri = "/XRMServices/2011/Discovery.svc";

        public CDSServerConnection(string serverAddress, string username, string password)
        {
            configuration = new CDSServerConfiguration();
            configuration.ServerAddress = serverAddress;
            configuration.Username = username;
            configuration.Password = password;
        }

        public static OrganizationServiceProxy GetOrganizationProxy(
            CDSServerConfiguration serverConfiguration)
        {
            if (serverConfiguration.OrganizationServiceManagement != null)
            {
                if (serverConfiguration.EndpointType != AuthenticationProviderType.ActiveDirectory)
                {
                    return GetProxy<IOrganizationService, OrganizationServiceProxy>(serverConfiguration);
                }
                else
                {
                    return new ManagedTokenOrganizationServiceProxy(
                        serverConfiguration.OrganizationServiceManagement,
                        serverConfiguration.Credentials);
                }
            }

            return GetProxy<IOrganizationService, OrganizationServiceProxy>(serverConfiguration);
        }

        public CDSServerConfiguration GetServerConfiguration()
        {
            bool IsO365Org = true;
            if (IsO365Org)
            {
                configuration.DiscoveryUri =
                    new Uri(String.Format("https://disco.{0}{1}",
                                        configuration.ServerAddress,
                                        partialDiscoveryUri));
                // usr pass
            }
            else
            {
                // server
                // port
                // ssl?
                // active directory
                // usr pass
                // Internet-facing deployment (IFD) 
                // domain configuration.DiscoveryUri = new Uri(String.Format("https://dev.{0}{1}", configuration.ServerAddress, partialDiscoveryUri));                                
                // domain configuration.DiscoveryUri = new Uri(String.Format("http[s]://{0}{1}", configuration.ServerAddress, partialDiscoveryUri));
                // oauth                
                // configuration.EndpointType = AuthenticationProviderType.ActiveDirectory;
                // configuration.EndpointType = AuthenticationProviderType.Federation;
                // configuration.EndpointType = AuthenticationProviderType.OnlineFederation;
            }

            GetOrganizations();
            configuration.OrganizationUri = GetOrganizationAddress(0);

            return configuration;
        }

        protected virtual void GetOrganizations()
        {
            using (DiscoveryServiceProxy serviceProxy = GetDiscoveryProxy())
            {
                if (serviceProxy != null)
                {
                    OrganizationDetailCollection orgs = DiscoverOrganizations(serviceProxy);
                    configuration.organizations = new OrganizationDetail[orgs.Count];
                    if (orgs.Count > 0)
                    {
                        for (int n = 0; n < orgs.Count; n++)
                        {
                            configuration.organizations[n] = orgs[n];
                        }
                    }
                }
                else
                {
                    try
                    {
                        throw new Exception();
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine("The application terminated with an error while GetOrganizations()");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        protected virtual Uri GetOrganizationAddress(int i)
        {
            string uri = "";
            configuration.organizations[i].Endpoints.TryGetValue(EndpointType.OrganizationService, out uri);
            return new Uri(uri);
        }

        private DiscoveryServiceProxy GetDiscoveryProxy()
        {
            try
            {
                DiscoveryServiceProxy discoveryProxy = GetProxy<IDiscoveryService, DiscoveryServiceProxy>(this.configuration);
                discoveryProxy.Execute(new RetrieveOrganizationsRequest());
                return discoveryProxy;
            }
            catch (System.ServiceModel.Security.SecurityAccessDeniedException ex)
            {
                // If authentication failed using current UserPrincipalName, 
                // request UserName and Password to try to authenticate using user credentials.
                if (!String.IsNullOrWhiteSpace(configuration.UserPrincipalName) &&
                    ex.Message.Contains("Access is denied."))
                {
                    configuration.AuthFailureCount += 1;
                }
                else
                {
                    throw ex;
                }
            }
            catch (System.ServiceModel.Security.MessageSecurityException ex) { }
            catch (System.ServiceModel.Security.SecurityNegotiationException ex) { }

            return GetProxy<IDiscoveryService, DiscoveryServiceProxy>(this.configuration);
        }

        /// <summary>
        /// Generic method to obtain discovery/organization service proxy instance.
        /// </summary>
        /// <typeparam name="TService">
        /// Set IDiscoveryService or IOrganizationService type 
        /// to request respective service proxy instance.
        /// </typeparam>
        /// <typeparam name="TProxy">
        /// Set the return type to either DiscoveryServiceProxy 
        /// or OrganizationServiceProxy type based on TService type.
        /// </typeparam>
        /// <param name="currentConfig">An instance of existing Configuration</param>
        /// <returns>An instance of TProxy 
        /// i.e. DiscoveryServiceProxy or OrganizationServiceProxy</returns>
        public static TProxy GetProxy<TService, TProxy>(CDSServerConfiguration currentConfig)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            Boolean isOrgServiceRequest = typeof(TService).Equals(typeof(IOrganizationService));

            Uri serviceUri = isOrgServiceRequest ?
                currentConfig.OrganizationUri : currentConfig.DiscoveryUri;

            IServiceManagement<TService> serviceManagement =
                (isOrgServiceRequest && currentConfig.OrganizationServiceManagement != null) ?
                (IServiceManagement<TService>)currentConfig.OrganizationServiceManagement :
                ServiceConfigurationFactory.CreateManagement<TService>(serviceUri);

            if (isOrgServiceRequest)
            {
                if (currentConfig.OrganizationTokenResponse == null)
                {
                    currentConfig.OrganizationServiceManagement =
                        (IServiceManagement<IOrganizationService>)serviceManagement;
                }
            }
            // Set the EndpointType in the current Configuration object 
            // while adding new configuration using discovery service proxy.
            else
            {
                currentConfig.EndpointType = serviceManagement.AuthenticationType;
                currentConfig.Credentials = GetUserLogonCredentials(currentConfig);
            }

            AuthenticationCredentials authCredentials = new AuthenticationCredentials();
            if (!String.IsNullOrWhiteSpace(currentConfig.UserPrincipalName))
            {
                // Single sing-on with the Federated Identity organization using current UserPrinicipalName.
                authCredentials.UserPrincipalName = currentConfig.UserPrincipalName;
            }
            else
            {
                authCredentials.ClientCredentials = currentConfig.Credentials;
            }

            Type classType;
            if (currentConfig.EndpointType !=
                AuthenticationProviderType.ActiveDirectory)
            {

                AuthenticationCredentials tokenCredentials =
                    serviceManagement.Authenticate(
                        authCredentials);

                if (isOrgServiceRequest)
                {
                    currentConfig.OrganizationTokenResponse = tokenCredentials.SecurityTokenResponse;
                    classType = typeof(ManagedTokenOrganizationServiceProxy);

                }
                else
                {
                    classType = typeof(ManagedTokenDiscoveryServiceProxy);
                }

                // Invokes ManagedTokenOrganizationServiceProxy or ManagedTokenDiscoveryServiceProxy 
                // (IServiceManagement<TService>, SecurityTokenResponse) constructor.
                return (TProxy)classType
                .GetConstructor(new Type[]
                    {
                        typeof(IServiceManagement<TService>),
                        typeof(SecurityTokenResponse)
                    })
                .Invoke(new object[]
                    {
                        serviceManagement,
                        tokenCredentials.SecurityTokenResponse
                    });
            }

            // Obtain discovery/organization service proxy for ActiveDirectory environment.
            if (isOrgServiceRequest)
            {
                classType = typeof(ManagedTokenOrganizationServiceProxy);
            }
            else
            {
                classType = typeof(ManagedTokenDiscoveryServiceProxy);
            }

            // Invokes ManagedTokenDiscoveryServiceProxy or ManagedTokenOrganizationServiceProxy 
            // (IServiceManagement<TService>, ClientCredentials) constructor.
            return (TProxy)classType
                .GetConstructor(new Type[]
                   {
                       typeof(IServiceManagement<TService>),
                       typeof(ClientCredentials)
                   })
               .Invoke(new object[]
                   {
                       serviceManagement,
                       authCredentials.ClientCredentials
                   });
        }

        /// <summary>
        /// Discovers the organizations that the calling user belongs to.
        /// </summary>
        /// <param name="service">A Discovery service proxy instance.</param>
        /// <returns>Array containing detailed information on each organization that 
        /// the user belongs to.</returns>
        public OrganizationDetailCollection DiscoverOrganizations(IDiscoveryService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
            RetrieveOrganizationsResponse orgResponse =
                (RetrieveOrganizationsResponse)service.Execute(orgRequest);

            return orgResponse.Details;
        }

        /// <summary>
        /// TODO: Salvage logic for various endpoint types
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ClientCredentials GetUserLogonCredentials(CDSServerConfiguration config)
        {
            ClientCredentials credentials = new ClientCredentials();
            String userName;
            SecureString password;
            String domain;
            Boolean isCredentialExist = (config.Credentials != null) ? true : false;
            switch (config.EndpointType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    // Uses credentials from windows credential manager for earlier saved configuration.
                    if (isCredentialExist && !String.IsNullOrWhiteSpace(config.OrganizationName))
                    {
                        domain = config.Credentials.Windows.ClientCredential.Domain;
                        userName = config.Credentials.Windows.ClientCredential.UserName;
                        if (String.IsNullOrWhiteSpace(config.Credentials.Windows.ClientCredential.Password))
                        {
                            password = ReadPassword();
                        }
                        else
                        {
                            password = config.Credentials.Windows.ClientCredential.SecurePassword;
                        }
                    }
                    else if (!isCredentialExist && !String.IsNullOrWhiteSpace(config.OrganizationName))
                    {
                        return null;
                    }
                    else
                    {
                        String[] domainAndUserName;
                        do
                        {
                            Console.Write("\nEnter domain\\username: ");
                            domainAndUserName = Console.ReadLine().Split('\\');
                            if (domainAndUserName.Length == 1 && String.IsNullOrWhiteSpace(domainAndUserName[0]))
                            {
                                return null;
                            }
                        }
                        while (domainAndUserName.Length != 2 || String.IsNullOrWhiteSpace(domainAndUserName[0])
                            || String.IsNullOrWhiteSpace(domainAndUserName[1]));

                        domain = domainAndUserName[0];
                        userName = domainAndUserName[1];

                        Console.Write("Enter Password: ");
                        password = ReadPassword();
                    }
                    if (null != password)
                    {
                        credentials.Windows.ClientCredential =
                            new System.Net.NetworkCredential(userName, password, domain);
                    }
                    else
                    {
                        credentials.Windows.ClientCredential = null;
                    }

                    break;
                // An internet-facing deployment (IFD) of Microsoft Dynamics CRM.          
                case AuthenticationProviderType.Federation:
                // Only one I support for now
                // Managed Identity/Federated Identity users using Microsoft Office 365.
                case AuthenticationProviderType.OnlineFederation:
                    if (isCredentialExist)
                    {
                        userName = config.Credentials.UserName.UserName;
                        password = ConvertToSecureString(config.Credentials.UserName.Password);
                    }
                    // For OnlineFederation environments, initially try to authenticate with the current UserPrincipalName
                    // for single sign-on scenario.
                    else if (config.EndpointType == AuthenticationProviderType.OnlineFederation
                        && config.AuthFailureCount == 0)
                    {
                        config.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;
                        return null;
                    }
                    else
                    {
                        config.UserPrincipalName = String.Empty;
                        userName = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(userName))
                        {
                            return null;
                        }

                        password = ReadPassword();
                    }
                    credentials.UserName.UserName = userName;
                    credentials.UserName.Password = ConvertToUnsecureString(password);
                    break;
                default:
                    credentials = null;
                    break;
            }
            return credentials;
        }
    }
}
