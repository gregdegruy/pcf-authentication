using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Configuration;
using System.ServiceModel;

namespace CDS.CustomControl.Encryption.Test.Authentication
{
    public class PowerApp
    {
        PowerAppServerConnection serverConnection;
        PowerAppServerConfiguration config;
        private AdvancedFind _advancedFind = new AdvancedFind();

        public PowerApp(PowerPlatformEnvironment environment) {
            serverConnection = new PowerAppServerConnection(environment.ServerAddress, environment.User, environment.Password);
            config = serverConnection.GetServerConfiguration();
        }

        public void Fetch()
        {                            
            Action action = () =>
            {
                OrganizationServiceProxy _serviceProxy;

                using (_serviceProxy = PowerAppServerConnection.GetOrganizationProxy(config))
                {
                    string userId = "98763981-12b8-44c5-a98c-1c1e392f56cf";
                    //var xml = "<fetch distinct='false' version='1.0' output-format='xml-platform' mapping='logical' no-lock='true'>" +
                    //            "<entity name='systemuser'>" +
                    //                "<attribute name='azureactivedirectoryobjectid' />" +
                    //                "<attribute name='systemuserid' />" +
                    //                "<attribute name='fullname' />" +
                    //                "<attribute name='internalemailaddress' />" +
                    //                "<filter type='and'>" +
                    //                    "<condition attribute='systemuserid' operator='eq' value='{" + userId + "}' />" +
                    //                "</filter>" +
                    //            "</entity>" +
                    //          "</fetch>";

                    var xml = "<fetch distinct='false' version='1.0' output-format='xml-platform' mapping='logical' no-lock='true'>" +
                            "<entity name='seismic_cc_configuration'>" +
                                "<attribute name='seismic_cc_configurationid' />" +
                                "<attribute name='seismic_name' />" +
                                "<attribute name='seismic_cc_clientid' />" +
                                "<attribute name='seismic_cc_clientsecret' />" +
                                "<filter type='and'>" +
                                    "<condition attribute='ownerid' operator='eq' uiname='Greg Degruy' uitype='systemuser' value='{" + userId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                          "</fetch>"; ;

                    Entity entity = _advancedFind.Fetch(xml, _serviceProxy);
                }
            };
            SafeExecutor(action);
        }

        public void GetSolutions()
        {
            Action action = () =>
            {
                OrganizationServiceProxy _serviceProxy;

                using (_serviceProxy = PowerAppServerConnection.GetOrganizationProxy(config))
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
