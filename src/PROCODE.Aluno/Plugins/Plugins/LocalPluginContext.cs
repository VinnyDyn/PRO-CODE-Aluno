using Microsoft.Xrm.Sdk;
using System;

namespace Plugin.Plugins
{
    public class LocalPluginContext
    {
        internal IServiceProvider ServiceProvider { get; private set; }
        internal IOrganizationService OrganizationService { get; private set; }
        internal IOrganizationService OrganizationServiceAdmin { get; private set; }
        internal IPluginExecutionContext PluginExecutionContext { get; private set; }
        internal IServiceEndpointNotificationService NotificationService { get; private set; }
        internal ITracingService TracingService { get; private set; }

        // -----------------------------------------------------------------------------------------------

        private LocalPluginContext() { }
        internal LocalPluginContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            this.NotificationService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            this.OrganizationService = factory.CreateOrganizationService(PluginExecutionContext.UserId);
            this.OrganizationServiceAdmin = factory.CreateOrganizationService(null);
        }

        // -----------------------------------------------------------------------------------------------

        internal void Trace(string message)
        {
            if (string.IsNullOrWhiteSpace(message) || TracingService == null)
            {
                return;
            }

            if (PluginExecutionContext == null)
            {
                TracingService.Trace(message);
            }
            else
            {
                TracingService.Trace(
                    "{0}, Correlation Id: {1}, Initiating User: {2}",
                    message,
                    PluginExecutionContext.CorrelationId,
                    PluginExecutionContext.InitiatingUserId);
            }
        }
        internal object InputParameters()
        {
            return PluginExecutionContext.InputParameters["Target"];
        }
        internal T GetPreImage<T>(string preImageName = "PreImage") where T : Entity
        {
            var entity = PluginExecutionContext.PreEntityImages[preImageName] as Entity;
            return entity.ToEntity<T>();
        }
        internal T GetTarget<T>() where T : Entity
        {
            var target = PluginExecutionContext.InputParameters["Target"] as Entity;

            return target.ToEntity<T>();
        }
        internal T GetPostImage<T>(string postImageName = "PostImage") where T : Entity
        {
            var image = PluginExecutionContext.PostEntityImages[postImageName] as Entity;

            return image.ToEntity<T>();
        }
        internal EntityReference GetEntityReference()
        {
            var target = PluginExecutionContext.InputParameters["Target"] as EntityReference;
            return target;
        }
        internal Relationship GetRelationship()
        {
            Relationship target = (Relationship)PluginExecutionContext.InputParameters["Relationship"];
            return target;
        }
        internal void AddSharedVariable(string key, object value)
        {
            this.PluginExecutionContext.SharedVariables.Add(key, value);
        }
        internal T GetSharedVariable<T>(string key)
        {
            return this.GetSharedVariableFromContext<T>(key, this.PluginExecutionContext);
        }
        private T GetSharedVariableFromContext<T>(string key, IPluginExecutionContext context)
        {
            T item = default(T);
            if (context != null && context.SharedVariables.Contains(key))
            {
                item = (T)context.SharedVariables[key];
            }
            return item;
        }
    }

}
