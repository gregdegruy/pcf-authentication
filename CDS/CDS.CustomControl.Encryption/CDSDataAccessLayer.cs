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
        private Entity RetrieveSystemUser(IOrganizationService service, Guid userId)
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