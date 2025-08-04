namespace proMX.ExamManagement.Wellknown
{
    internal class QuestionPaper
    {
        public const string LogicalName = "dc_questionpaper";
        //public const string Id = "dc_examquestionid"; // Lookup to ExamQuestion
        //public const string Question = "dc_questionid"; // Lookup to Question
        public const string ExamName = "dc_examid"; // Lookup to Exam table to get the exam name
        public const string QuestionPaperIdNum = "dc_questionpaperidnum"; // Autonumber
        public const string QuestionPaperName = "dc_examquestion1"; // name of the question paper e.g. "IOT exam QP1"
        public const string QuestionSetName = "dc_questionname"; // lookup to Question table
        public const string MonthAndYear = "dc_monthandyear"; // month and year of the question paper (option set)

        public enum QuestionPaper_MonthAndYear
        {           
            May2023 = 10,
            December2023 = 20,
            May2024 = 30,
            December2024 = 40,
            May2025 = 50,
            December2025 = 60,
            May2026 = 70,
            December2026 = 80,
        }
    }
}
