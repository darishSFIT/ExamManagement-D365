using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using proMX.ExamManagement.Plugins.Wellknown;
using proMX.ExamManagement.Wellknown;
using System;
using System.ServiceModel;

namespace proMX.ExamManagement.Plugins
{
    /// <summary>
    /// On create/update of result, calculate GPA based on marks obtained and total marks
    /// Formula: GPA = (Marks Obtained / Total Marks) * 10
    /// </summary>
    /// <remarks>
    /// 02.08.2025 - Darish: Created
    /// </remarks>
    /// <registration>
    /// Registration:
    /// On create/update of Result, Post Operation, Sync
    /// </registration>
    public class Result_CreateUpdate_CalculateGPA : IPlugin
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
                if (message != "create" && message != "update")
                {
                    tracingService.Trace("Context message is not create or update");
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

        private void Implementation(IOrganizationService service, Entity targetEntity, ITracingService tracingService,
            IPluginExecutionContext context, string message)
        {
            try
            {
                // Get the actual created/updated record ID
                Guid recordId = Guid.Empty;

                if (message == "create" && context.OutputParameters.Contains("id"))
                {
                    recordId = (Guid)context.OutputParameters["id"];
                }
                else if (message == "update")
                {
                    recordId = targetEntity.Id;
                }

                tracingService.Trace($"Record ID: {recordId}");

                // Check if marks obtained or total marks fields are being updated
                bool shouldCalculateGPA = targetEntity.Contains(Result.MarksObtained) || targetEntity.Contains(Result.TotalMarks);

                if (!shouldCalculateGPA)
                {
                    tracingService.Trace("Neither MarksObtained nor TotalMarks fields are being updated");
                    return;
                }

                // For update operation, we need to get the full record to have all field values
                Entity fullResultRecord = null;
                if (message == "update")
                {
                    fullResultRecord = service.Retrieve(
                        Result.LogicalName, 
                        recordId,
                        new ColumnSet(Result.MarksObtained, Result.TotalMarks, Result.GPA));
                    tracingService.Trace("Retrieved full result record for update operation");
                }

                // Get marks obtained and total marks values
                decimal marksObtained = 0;
                decimal totalMarks = 0;

                if (message == "create")
                {
                    // For create, get values from target entity
                    marksObtained = targetEntity.GetAttributeValue<int>(Result.MarksObtained);
                    totalMarks = targetEntity.GetAttributeValue<int>(Result.TotalMarks);
                }
                else
                {
                    // For update, get values from full record or target entity if updated
                    marksObtained = targetEntity.Contains(Result.MarksObtained)
                        ?targetEntity.GetAttributeValue<int>(Result.MarksObtained)
                        :fullResultRecord.GetAttributeValue<int>(Result.MarksObtained);

                    totalMarks = targetEntity.Contains(Result.TotalMarks)
                        ?targetEntity.GetAttributeValue<int>(Result.TotalMarks)
                        :fullResultRecord.GetAttributeValue<int>(Result.TotalMarks);
                }

                tracingService.Trace($"Marks Obtained: {marksObtained}, Total Marks: {totalMarks}");

                // Validate that total marks is greater than 0 to avoid division by zero
                if (totalMarks <= 0)
                {
                    tracingService.Trace("Total marks is 0 or negative. Cannot calculate GPA.");
                    return;
                }

                // Validate that marks obtained is not negative or greater than total marks
                if (marksObtained < 0)
                {
                    tracingService.Trace("Marks obtained is negative. Setting GPA to 0.");
                    marksObtained = 0;
                }
                else if (marksObtained > totalMarks)
                {
                    tracingService.Trace("Marks obtained is greater than total marks. Setting marks obtained to total marks.");
                    marksObtained = totalMarks; //avoid overflow in GPA calculation
                }

                // Calculate GPA using the formula: GPA = (Marks Obtained / Total Marks) * 10
                decimal gpa = (marksObtained/totalMarks)*10;
                
                tracingService.Trace($"Calculated GPA: {gpa}");

                // Update the result record with the calculated GPA
                Entity resultUpdate = new Entity(Result.LogicalName, recordId);
                resultUpdate[Result.GPA] = gpa;
                if (gpa >= 9.0m)
                {
                    resultUpdate[Result.Grade] = new OptionSetValue((int)Result.Result_Grade.A); // Assuming 121750000 is the OptionSet value for "First Class with Distinction"
                    resultUpdate[Result.ResultName] = "Excellent"; // Set Result Name for FCD
                }
                else if (gpa >= 7.5m)
                {
                    resultUpdate[Result.Grade] = new OptionSetValue((int)Result.Result_Grade.B); // Assuming 121750000 is the OptionSet value for "First Class with Distinction"
                    resultUpdate[Result.ResultName] = "First Class Distinction";
                }
                else if (gpa >= 5.0m)
                {
                    resultUpdate[Result.Grade] = new OptionSetValue((int)Result.Result_Grade.C);
                    resultUpdate[Result.ResultName] = "Second Class Distinction";
                }
                else if (gpa >= 3.5m)
                {
                    resultUpdate[Result.Grade] = new OptionSetValue((int)Result.Result_Grade.D);
                    resultUpdate[Result.ResultName] = "Pass Class";
                }
                else
                {
                    resultUpdate[Result.Grade] = new OptionSetValue((int)Result.Result_Grade.F);
                    resultUpdate[Result.ResultName] = "Fail";
                }
                
                service.Update(resultUpdate);

                tracingService.Trace("Successfully calculated and updated GPA");
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error in Implementation: {ex.Message}");
                throw;
            }
        }
    }
}
