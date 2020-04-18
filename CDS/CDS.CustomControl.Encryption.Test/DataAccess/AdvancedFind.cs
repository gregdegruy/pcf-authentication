using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace CDS.CustomControl.Encryption.Test
{ 
    public class AdvancedFind : IAdvancedFind
    {
        public AdvancedFind() { }

        public Entity Fetch(string xml, IOrganizationService organizationService)
        {
            RetrieveMultipleRequest retrieveMultipleRequest
                = new RetrieveMultipleRequest() { Query = new FetchExpression(xml) };

            EntityCollection entityCollection = ((RetrieveMultipleResponse)organizationService
                .Execute(retrieveMultipleRequest))
                .EntityCollection;

            if (entityCollection != null
                && entityCollection.Entities.Count > 0
                && entityCollection[0] != null)
            {
                return entityCollection[0];
            }
            else
            { return null; }
        }
    }
}
