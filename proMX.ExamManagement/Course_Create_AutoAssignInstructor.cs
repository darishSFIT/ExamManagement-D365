using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using proMX.ExamManagement.Plugins.Wellknown;
using proMX.ExamManagement.Wellknown;
using System;
using System.ServiceModel;

namespace proMX.ExamManagement.Plugins
{
    /// <summary>
    /// On create of course, auto-assign instructor based on department option set
    /// </summary>
    /// <remarks>
    /// 02.08.2025 - Darish: Created
    /// </remarks>
    /// <registration>
    /// Registration:
    /// On create of Course, Post Operation, Sync
    /// </registration>
    public class Course_Create_AutoAssignInstructor : IPlugin
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
                // Get department option set value from course
                if (targetEntity.Contains(Course.Department))
                {
                    var departmentOptionSet = targetEntity.GetAttributeValue<OptionSetValue>(Course.Department);
                    if (departmentOptionSet != null)
                    {
                        int departmentValue = departmentOptionSet.Value;
                        tracingService.Trace($"Department option set value found: {departmentValue}");

                        // Query Instructor entity for instructor with matching department option set
                        QueryExpression query = new QueryExpression(Instructor.LogicalName)
                        {
                            ColumnSet = new ColumnSet(Instructor.Id, Instructor.InstructorName, Instructor.Department),
                            Criteria = new FilterExpression
                            {
                                Conditions =
                                {
                                    new ConditionExpression(Instructor.Department, ConditionOperator.Equal, departmentValue)
                                }
                            },
                            TopCount = 1 // Get only first instructor
                        };

                        var results = service.RetrieveMultiple(query);

                        if (results.Entities.Count > 0)
                        {
                            var instructor = results.Entities[0];
                            var instructorRef = instructor.ToEntityReference();

                            tracingService.Trace($"Found instructor: {instructor.GetAttributeValue<string>(Instructor.InstructorName)}");

                            // Update course with assigned instructor
                            Entity courseUpdate = new Entity(Course.LogicalName, targetEntity.Id);
                            courseUpdate[Course.InstructorName] = instructorRef;
                            service.Update(courseUpdate);

                            tracingService.Trace("Successfully assigned instructor to course");
                        }
                        else
                        {
                            tracingService.Trace($"No instructor found for department option set value: {departmentValue}");
                        }
                    }
                    else
                    {
                        tracingService.Trace("Department option set field is empty");
                    }
                }
                else
                {
                    tracingService.Trace("Course does not contain department field");
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error in Implementation: {ex.Message}");
                throw;
            }
        }
    }
}
