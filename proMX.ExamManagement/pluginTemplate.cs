using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using proMX.ExamManagement.Plugins.Wellknown;
using System;
using System.ServiceModel;

namespace proMX.ExamManagement.Plugins
{
    public class EntityName_Message_Action : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                Console.WriteLine(Course.LogicalName); // Fixed: Replaced 'System.Writeline' with 'Console.WriteLine'
                tracingService.Trace("Start Execute method");

                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                {
                    tracingService.Trace("Input Parameter doesn't contain Target or the Target isn't an Entity.");
                    return;
                }
                
                string message = context.MessageName.ToLower();

                if (message != "create" && message != "update") // message != "delete" && message != "assign" && message != "setstate")
                {
                    tracingService.Trace("Context message is not create or update"); // , delete, assign, setstate
                    return;
                }

                Entity targetEntity = context.InputParameters["Target"] as Entity;
                tracingService.Trace($"Entity : {targetEntity.LogicalName}");
                Implementation(service, targetEntity, tracingService, context, message);
                tracingService.Trace("Execute/Main function ended");

            }
            catch (InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException("Execute: " + (ex.InnerException != null &&
                    ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message));
            }
            catch (FaultException ex)
            {
                throw new InvalidPluginExecutionException("Execute: " + (ex.InnerException != null &&
                    ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message));
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Execute: " + (ex.InnerException != null &&
                    ex.InnerException.Message != null ? ex.InnerException.Message : ex.Message));
            }
        }
        private void Implementation(IOrganizationService service, Entity targetEntity, ITracingService tracingService, IPluginExecutionContext context, string message) // remove 'context' and 'message' if not needed
        {
            try
            {
                tracingService.Trace("Implementation logic executed successfully.");
                // main code
            }
            catch (Exception ex)
            {
                tracingService.Trace("Error in Implementation: " + ex.Message);
                throw;
            }
        }
    }
}
