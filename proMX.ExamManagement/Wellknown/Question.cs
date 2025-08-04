namespace proMX.ExamManagement.Plugins.Wellknown
{
    internal class Question
    {
        public const string LogicalName = "dc_question";
        public const string Id = "dc_questionid"; // GUID
        public const string ExamLookup = "dc_examid"; // Lookup to Exam
        public const string QuestionSet = "dc_question1"; // question header like Q.1 or Q.2, etc.
        public const string QuestionText = "dc_questiontext"; // actual question text
        public const string QuestionIdNum = "dc_questionidnum"; // Autonumber
        public const string Marks = "dc_marks"; // marks per question
        public const string IsActive = "dc_isactive"; // Boolean
        public enum Question_IsActive
        {
            True = 0,
            False = 1
        }
    }
}