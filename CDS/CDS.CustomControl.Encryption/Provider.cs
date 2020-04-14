using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

            if (cdsUser.ContainsKey("fullname")) {
                message.Append(cdsUser["fullname"] + " completed the ");
            }
            else { message.Append("USERNAME ERROR completed the "); }

            string email = "";
            string token = "";
            if (cdsUser.ContainsKey("internalemailaddress"))
            { email = cdsUser["internalemailaddress"]; }
                
            if (cdsUser.ContainsKey("azureactivedirectoryobjectid"))
            { token = cdsUser["azureactivedirectoryobjectid"]; }

            passThroughOnlyTracingService.Trace("Message " + message.ToString());

            FormUrlEncodedContent formUrlEncodedContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("SHA", "token"),
                new KeyValuePair<string, string>("Message", message.ToString())
            });                

            //var response = new HttpResponseMessage();
            //response = await httpClient.PostAsync("api/cds/authentication/token", formUrlEncodedContent);
            //response.EnsureSuccessStatusCode();
        }            
    }
}