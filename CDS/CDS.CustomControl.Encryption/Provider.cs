using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace CDS.CustomControl.Encryption
{
    internal class Provider : IProvider
    {
        private IExecutionContext passThroughOnlyExecutionContext;
        private IOrganizationService passThroughOnlyOrganizationService;
        private ITracingService passThroughOnlyTracingService;

        private static HttpClient httpClient = new HttpClient();

        public Provider(IExecutionContext context, IOrganizationService service, ITracingService logs)
        {
            passThroughOnlyExecutionContext = context;
            passThroughOnlyOrganizationService = service;
            passThroughOnlyTracingService = logs;
        }

        public async void NotifyEvent()
        {
            passThroughOnlyTracingService.Trace("In NotifyEvent");

            var cdsDataAccessLayer = new CDSDataAccessLayer(
                passThroughOnlyExecutionContext, passThroughOnlyOrganizationService, passThroughOnlyTracingService);

            passThroughOnlyTracingService.Trace("Getting cds system user data");
            Dictionary<string, dynamic> cdsUser = cdsDataAccessLayer.GetCDSUserData();

            // TODO: handle dynamics regions in URL
            // emea = crm4.dynamics.com, na = crm.dynamics.com AND https://docs.microsoft.com/en-us/dynamics365/customer-engagement/admin/manage-user-account-synchronization
            httpClient.BaseAddress = new Uri("https://org.api.dynamics.com/");

            var message = new StringBuilder("");
            if (cdsUser.ContainsKey("fullname"))
            {
                message.Append(cdsUser["fullname"] + " completed the ");
            } else { 
                message.Append("USERNAME ERROR completed the "); 
            }
            string email = "";
            string token = "";
            if (cdsUser.ContainsKey("internalemailaddress")) { 
                email = cdsUser["internalemailaddress"]; 
            }
            if (cdsUser.ContainsKey("azureactivedirectoryobjectid")) { 
                token = cdsUser["azureactivedirectoryobjectid"]; 
            }

            passThroughOnlyTracingService.Trace("Message " + message.ToString());

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("SHA", "token"),
                new KeyValuePair<string, string>("Message", message.ToString())
            });
           
            string azFuncResponse = "Waiting for azFuncResponse...";
            const string azureFunctionUrl = "https://cdscryptography.azurewebsites.net/api/HttpTriggerRestSharp";

            if (passThroughOnlyExecutionContext.MessageName == "seismic_cc_login_action")
            {
                Dictionary<string, string> loginCredentials = cdsDataAccessLayer.GetLoginCredentials();
                passThroughOnlyTracingService.Trace("Filled dictionary");
                passThroughOnlyTracingService.Trace("Username " + loginCredentials["username"]);
                passThroughOnlyTracingService.Trace("Password " + loginCredentials["password"]);

                Entity configuration = cdsDataAccessLayer.RetrieveConfiguration();
                passThroughOnlyTracingService.Trace("Got config");
                passThroughOnlyTracingService.Trace("client id " + configuration.Attributes["seismic_cc_clientid"]);
                passThroughOnlyTracingService.Trace("client secret " + configuration.Attributes["seismic_cc_clientsecret"]);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(azureFunctionUrl);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                string json = "";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    json =
                    '{' +
                        "\"client_id\" : \"" + configuration.Attributes["seismic_cc_clientid"] + "\"," +
                        "\"client_secret\" : \"" + configuration.Attributes["seismic_cc_clientsecret"] + "\"," +
                        "\"username\" : \"" + loginCredentials["username"] + "\"," +
                        "\"password\" : \"" + loginCredentials["password"] + "\"" +
                    '}';

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var responseText = streamReader.ReadToEnd();
                        azFuncResponse = responseText;                        
                    }
                    var cols
                            = new ColumnSet(new String[] { "systemuserid", "internalemailaddress", "seismic_cc_token" });
                    Entity systemUser
                        = passThroughOnlyOrganizationService.Retrieve("systemuser", passThroughOnlyExecutionContext.InitiatingUserId, cols);
                    systemUser["seismic_cc_token"] = azFuncResponse;
                    systemUser["seismic_cc_username"] = loginCredentials["username"];
                    passThroughOnlyTracingService.Trace("About to update system user id: " + passThroughOnlyExecutionContext.InitiatingUserId.ToString());
                    passThroughOnlyTracingService.Trace("Using this new token: " + systemUser["seismic_cc_token"]);
                    try
                    {
                        passThroughOnlyOrganizationService.Update(systemUser);
                    }
                    catch (Exception e) { passThroughOnlyTracingService.Trace("ERROR: could not update user: " + e.ToString()); }
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            passThroughOnlyTracingService.Trace("Token " + azFuncResponse);
        }
    }
}
