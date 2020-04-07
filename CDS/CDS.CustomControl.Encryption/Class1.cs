using System;
using Microsoft.Xrm.Sdk;

namespace CDS.CustomControl.Encryption
{
    public class EventTrigger : IPlugin
    {
        /// <summary>
        /// 
        /// Captures completed Task trigger and sends data to SpurGo
        /// 
        /// Trigger: Completed a task eg. Create an Account
        /// Stage: Pre
        /// Method: Synchronous
        /// Filter Attributes: none
        /// Pre Attributes: none
        /// Post Attributes: none
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory organizationServiceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService organizationService =
                organizationServiceFactory.CreateOrganizationService(null);

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracingService.Trace("Event Trigger running...");

            Provider externalProvider = new Provider(context, organizationService, tracingService);
            Provider.taskNotifyEvent();
        }
    }
}
