using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using proMX.ExamManagement.Plugins.Wellknown;
using proMX.ExamManagement.Wellknown;

namespace proMX.ExamManagement.Plugins
{
    ///
    /// On create of Exam, auto-create Question Paper and link 5 random Questions for the same course
    ///
    /// 02.08.2025 - Darish: Created
    ///
    ///
    /// Registration:
    /// On create of Exam, Post Operation, Sync
    ///
    public class Exam_Create_AutoCreateQuestionPaper : IPlugin
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

                if (context.MessageName.ToLower() != "create")
                {
                    tracingService.Trace("Context message is not create");
                    return;
                }

                Entity exam = (Entity)context.InputParameters["Target"];
                tracingService.Trace($"Entity : {exam.LogicalName}");

                Implementation(service, exam, tracingService, context);

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
        private void Implementation(IOrganizationService service, Entity exam, ITracingService tracingService, IPluginExecutionContext context)
        {
            try
            {
                Guid examId = Guid.Empty;
                if (context.OutputParameters.Contains("id"))
                {
                    examId = (Guid)context.OutputParameters["id"];
                }

                tracingService.Trace($"Exam Id: {examId}");

                if (examId == Guid.Empty)
                {
                    tracingService.Trace("Exam Id is empty. Cannot create Question Paper.");
                    return;
                }

                string examName = exam.GetAttributeValue<string>(Exam.ExamName) ?? "Exam";

                // Retrieve Course linked to Exam
                EntityReference courseRef = exam.GetAttributeValue<EntityReference>(Exam.CourseName);
                if (courseRef == null)
                {
                    tracingService.Trace("Exam does not have a linked Course.");
                    return;
                }

                tracingService.Trace($"Course linked to Exam: {courseRef.Id}");

                // Create Question Paper record
                Entity questionPaper = new Entity(QuestionPaper.LogicalName);
                questionPaper[QuestionPaper.QuestionPaperName] = $"{examName} QP {DateTime.Now:dd/MM/yyyy}";
                questionPaper[QuestionPaper.ExamName] = new EntityReference(Exam.LogicalName, examId);

                Guid questionPaperId = service.Create(questionPaper);
                tracingService.Trace($"Question Paper created with Id: {questionPaperId}");

                // Retrieve active Questions linked to this exam
                QueryExpression query = new QueryExpression(Question.LogicalName)
                {
                    ColumnSet = new ColumnSet(Question.Id, Question.QuestionText, Question.Marks /*, Question.IsActive*/),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            //new ConditionExpression(Question.IsActive, ConditionOperator.Equal, true) ,
                            new ConditionExpression(Question.CourseLookup, ConditionOperator.Equal, courseRef.Id)
                        }
                    }
                };
                tracingService.Trace("filter vlaues "+Question.CourseLookup + " condition set to: " + courseRef.Id);
                tracingService.Trace("active question boolean set: "+Question.Question_IsActive.True);

                var questionResults = service.RetrieveMultiple(query);
                var questions = questionResults.Entities.ToList();

                if (!questions.Any())
                {
                    tracingService.Trace("No active Questions found for the Exam.");
                    return;
                }

                // Select 5 random questions (or less if not available)
                var rnd = new Random();
                var selectedQuestions = questions.OrderBy(x => rnd.Next()).Take(5).ToList();

                tracingService.Trace($"Selected {selectedQuestions.Count} Questions for the Question Paper.");

                // Create collection of question references
                var questionRefs = new EntityReferenceCollection();
                foreach (var question in selectedQuestions)
                {
                    questionRefs.Add(question.ToEntityReference());
                }

                // Define the relationship schema name - replace with your actual N:N relationship name between questionpaper and question
                string relationshipName = "dc_ExamQuestion_dc_Question_dc_QuestionRelatedForm"; // Example: Schema name of N:N relationship

                // Create the relationship object
                var relationship = new Relationship(relationshipName);

                // Associate the questions with the question paper
                service.Associate(QuestionPaper.LogicalName, questionPaperId, relationship, questionRefs);

                tracingService.Trace("Successfully associated Questions with the Question Paper.");
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error in Implementation: {ex.Message}");
                throw;
            }
        }

    }
}
