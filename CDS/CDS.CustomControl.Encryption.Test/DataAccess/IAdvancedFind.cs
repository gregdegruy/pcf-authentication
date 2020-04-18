using Microsoft.Xrm.Sdk;

namespace CDS.CustomControl.Encryption.Test
{
    interface IAdvancedFind
    {
        Entity Fetch(string xml, IOrganizationService organizationService);
    }
}
