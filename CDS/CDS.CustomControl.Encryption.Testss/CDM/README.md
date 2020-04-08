`CrmSvcUtil.exe /url:https://<serverName>/<organizationName>/XRMServices/2011/Organization.svc /out:<outputFilename>.cs /username:<username> /password:<password> /domain:<domainName> /namespace:<outputNamespace> /serviceContextName:<serviceContextName>`

`CrmSvcUtil.exe /url:https://openhack.api.crm.dynamics.com/XRMServices/2011/Organization.svc /out:CDSEntities.cs /username:grdegr@grdegr.onmicrosoft.com /password:<add_yours> /namespace:CDS.CustomControl.Encryption.Test.Entities`

Follow this [doc](https://docs.microsoft.com/en-us/dynamics365/customerengagement/on-premises/developer/org-service/create-early-bound-entity-classes-code-generation-tool) to generate the types.

Create early bound entity classes with the code generation tool (CrmSvcUtil.exe)
CrmSvcUtil.exe is a command-line code generation tool for use with Dynamics 365 for Customer Engagement. 
This tool generates early-bound .NET Framework classes that represent the entity data model used by Dynamics 365 for Customer Engagement.
