using CDS.CustomControl.Encryption.Test.Authentication;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using Xunit;

namespace CDS.CustomControl.Encryption.Test
{
    public class IntegrationTest
    {
        CDSServerConnection serverConnection;
        CDSServerConfiguration config;

        private String _discoveryServiceAddress = "https://dev.crm.dynamics.com/XRMServices/2011/Discovery.svc";
        private String _organizationUniqueName = "orgf8c684be";
        private String _userName = AppConfig.USER;
        private String _password = AppConfig.PASSWORD;

        [Fact]
        public void CDSAuthentication()
        {
            serverConnection = new CDSServerConnection(AppConfig.CDS_SERVER, AppConfig.USER, AppConfig.PASSWORD);
            config = serverConnection.GetServerConfiguration();

            Action action = () =>
            {
                OrganizationServiceProxy _serviceProxy;

                using (_serviceProxy = CDSServerConnection.GetOrganizationProxy(config))
                {
                    QueryExpression querySampleSolution = new QueryExpression
                    {
                        EntityName = Solution.EntityLogicalName,
                        ColumnSet = new ColumnSet(new string[] { "publisherid", "installedon", "version", "versionnumber", "friendlyname" }),
                        Criteria = new FilterExpression()
                    };

                    querySampleSolution.Criteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
                    DataCollection<Entity> solutions = _serviceProxy.RetrieveMultiple(querySampleSolution).Entities;
                    Console.ReadLine();
                }
            };
            SafeExecutor(action);
        }

        private void SafeExecutor(Action action)
        {
            SafeExecutor(() => { action(); return 0; });
        }

        private T SafeExecutor<T>(Func<T> action)
        {
            try
            {
                return action();
            }

            #region exceptions
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            #endregion

            finally
            {
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }

            return default(T);
        }
    }
}
