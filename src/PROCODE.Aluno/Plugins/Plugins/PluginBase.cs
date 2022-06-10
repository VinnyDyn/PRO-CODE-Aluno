using Microsoft.Xrm.Sdk;
using System;
using System.Globalization;
using System.ServiceModel;

namespace Plugin.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        protected string ChildClassName { get; private set; }
        internal PluginBase(Type childClassName)
        {
            ChildClassName = childClassName.ToString();
        }
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            LocalPluginContext localcontext = new LocalPluginContext(serviceProvider);


            localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.ChildClassName));
            try
            {
                ExecuteCrmPlugin(localcontext);
                return;
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));
                throw;
            }
            finally
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.ChildClassName));
            }
        }
        protected virtual void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {

        }
    }

}
