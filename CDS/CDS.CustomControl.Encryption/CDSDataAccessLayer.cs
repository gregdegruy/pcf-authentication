using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace CDS.CustomControl.Encryption
{
    internal class CDSDataAccessLayer
    {
        private IExecutionContext executionContext;
        private IOrganizationService organizationService;
        private ITracingService tracingService;

        private AdvancedFind _advancedFind = new AdvancedFind();
        private Dictionary<string, dynamic> cdsSystemUserData;

        public CDSDataAccessLayer(IExecutionContext executionContext,
            IOrganizationService organizationService, ITracingService tracingService)
        {
            this.executionContext = executionContext;
            this.organizationService = organizationService;
            this.tracingService = tracingService;
        }

        public Dictionary<string, dynamic> GetCDSUserData()
        {
            Entity systemUser = RetrieveSystemUser(organizationService, executionContext.InitiatingUserId);

            if (systemUser != null)
            {
                cdsSystemUserData = new Dictionary<string, dynamic>();
                foreach (var attribute in systemUser.Attributes)
                {
                    tracingService.Trace("systemUser attr is " + attribute.ToString());
                    cdsSystemUserData.Add(attribute.Key, attribute.Value.ToString());
                }

                return cdsSystemUserData;
            }

            return new Dictionary<string, dynamic>() { { "", "" } };
        }

        public Dictionary<string, string> GetLoginCredentials()
        {
            if (executionContext.InputParameters.Count > 0 
                && executionContext.MessageName == "seismic_cc_login_action")
            {
                var externelUserCredentials = new Dictionary<string, string>() 
                { 
                    { "username", executionContext.InputParameters["username"].ToString() },
                    { "password", executionContext.InputParameters["password"].ToString() }
                };

                return externelUserCredentials;
            }

            return new Dictionary<string, string>() { { "", "" } };
        }

        public Entity RetrieveConfiguration()
        {
            var xml = "<fetch distinct='false' version='1.0' output-format='xml-platform' mapping='logical' no-lock='true'>" +
                            "<entity name='seismic_cc_configuration'>" +
                                "<attribute name='seismic_cc_configurationid' />" +
                                "<attribute name='seismic_name' />" +
                                "<attribute name='seismic_cc_clientid' />" +
                                "<attribute name='seismic_cc_clientsecret' />" +
                                "<filter type='and'>" +
                                    "<condition attribute='ownerid' operator='eq' uiname='Greg Degruy' uitype='systemuser' value='{" + executionContext.InitiatingUserId.ToString() + "}' />" +
                                "</filter>" +
                            "</entity>" +
                          "</fetch>"; ;

            Entity configuration = _advancedFind.Fetch(xml, this.organizationService);

            if (configuration != null)
            {
                return configuration;
            }
            else { return null; }
        }

        public Entity RetrieveSystemUser(IOrganizationService service, Guid userId)
        {
            var xml = "<fetch distinct='false' version='1.0' output-format='xml-platform' mapping='logical' no-lock='true'>" +
                            "<entity name='systemuser'>" +
                                "<attribute name='azureactivedirectoryobjectid' />" +
                                "<attribute name='systemuserid' />" +
                                "<attribute name='fullname' />" +
                                "<attribute name='internalemailaddress' />" +
                                "<filter type='and'>" +
                                    "<condition attribute='systemuserid' operator='eq' value='{" + userId.ToString() + "}' />" +
                                "</filter>" +
                            "</entity>" +
                          "</fetch>";

            Entity systemUser = _advancedFind.Fetch(xml, service);

            if (systemUser != null)
            {
                return systemUser;
            }
            else { return null; }
        }
    }
}