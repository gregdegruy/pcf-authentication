using Microsoft.Xrm.Sdk;

namespace CDS.CustomControl.Encryption
{
    interface IAdvancedFind
    {
        Entity Fetch(string xml, IOrganizationService organizationService);
    }
}
