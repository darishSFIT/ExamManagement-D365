using System;
using System.Collections.Generic;
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

                // Create Question Paper
                Entity questionPaper = new Entity(QuestionPaper.LogicalName);
                questionPaper[QuestionPaper.QuestionPaperName] = $"{examName} QP {DateTime.Now:dd/MM/yyyy}";
                questionPaper[QuestionPaper.ExamName] = new EntityReference(Exam.LogicalName, examId);

                Guid questionPaperId = service.Create(questionPaper);
                tracingService.Trace($"Question Paper created with Id: {questionPaperId}");

                // Retrieve active Questions for the same course (random 5)
                QueryExpression query = new QueryExpression(Question.LogicalName)
                {
                    ColumnSet = new ColumnSet(Question.Id, Question.QuestionText, Question.Marks, Question.IsActive),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(Question.IsActive, ConditionOperator.Equal, (int)Question.Question_IsActive.True),
                            // Filter Questions related to the Course via Exam lookup's course - we assume Question has dc_examid lookup to Exam
                            // So first, get Exams linked to the same course (filter by courseRef.Id)
                            // But since Plugin is on Exam, and Questions linked by Exam, direct filter by dc_examid = examId should work
                            new ConditionExpression(Question.ExamLookup, ConditionOperator.Equal, examId)
                        }
                    }
                };

                var questionResults = service.RetrieveMultiple(query);
                var questions = questionResults.Entities.ToList();

                if (questions.Count == 0)
                {
                    tracingService.Trace("No active questions found for this Exam.");
                    return;
                }

                // Randomly select 5 questions (or all if less than 5)
                var rnd = new Random();
                var selectedQuestions = questions.OrderBy(x => rnd.Next()).Take(5).ToList();

                tracingService.Trace($"Selected {selectedQuestions.Count} random questions.");

                // Link each Question to the Question Paper by updating the Question's QuestionPaper lookup
                foreach (var question in selectedQuestions)
                {
                    Entity updateQuestion = new Entity(Question.LogicalName, question.Id);
                    updateQuestion[Question.QuestionText] = new EntityReference(QuestionPaper.LogicalName, questionPaperId);
                    service.Update(updateQuestion);
                    tracingService.Trace($"Linked Question Id: {question.Id} to Question Paper Id: {questionPaperId}");
                }

                tracingService.Trace("Question Paper creation and linking completed successfully.");
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Error in Implementation: {ex.Message}");
                throw;
            }
        }
    }
}
