using Microsoft.Xrm.Sdk;

namespace Plugin.Business
{
    public abstract class BusinessBase
    {
        protected ITracingService TracingService = null;
        protected IOrganizationService Service { get; }
        protected IOrganizationService ServiceAdmin { get; }

        protected BusinessBase(IOrganizationService service, IOrganizationService serviceAdmin, ITracingService tracingService)
        {
            Service = service;
            ServiceAdmin = serviceAdmin;
            TracingService = tracingService;
        }
    }
}
