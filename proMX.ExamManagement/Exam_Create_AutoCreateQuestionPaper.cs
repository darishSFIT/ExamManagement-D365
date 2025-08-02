using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
namespace proMX.ExamManagement
{
    internal class Exam_Create_AutoCreateQuestionPaper : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                tracingService.Trace("Start Execute method");
                
                if (!context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                {
                    tracingService.Trace("Input Parameter doesn't contain Target or the Target isn't an Entity.");
                    return;
                }

                string message = context.MessageName.ToLower();
                if (message != "create")
                {
                    tracingService.Trace("Context message is not create");
                    return;
                }

                Entity targetEntity = context.InputParameters["Target"] as Entity;
                tracingService.Trace($"Entity : {targetEntity.LogicalName}");
                Implementation(service, targetEntity, tracingService);
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
        private void Implementation(IOrganizationService service, Entity targetEntity, ITracingService tracingService)
        {
            try
            {

            }
            catch (Exception ex)
            {
                tracingService.Trace("Error in Implementation: " + ex.Message);
                throw;
            }

        }
    }
}
