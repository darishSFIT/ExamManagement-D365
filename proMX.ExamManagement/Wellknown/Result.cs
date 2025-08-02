namespace proMX.ExamManagement.Wellknown
{
    internal class Result
    {
        public const string LogicalName = "dc_result";
        public const string Id = "dc_marksobtained"; // GUID
        public const string CourseName = "dc_coursename"; // Lookup to Course
        public const string ExamName = "dc_examid"; // Lookup to Exam
        public const string GPA = "dc_cgpa"; // Decimal field for CGPA
        public const string MarksObtained = "dc_marksobtained"; // Decimal field for Marks Obtained
        public const string TotalMarks = "dc_totalmarks"; // Decimal field for Total Marks
        public const string ResultIdNum = "dc_resultidnum"; // Autonumber for Result ID
        public const string ResultName = "dc_result1"; // Text field for Result Name
        public const string StudentId = "dc_studentid"; // Lookup to Student
        public const string Grade = "dc_grade"; // OptionSet for Grade
        public enum Result_Grade
        {
            A = 100, // Assuming this is the OptionSet value for "First Class with Distinction"
            B = 200, // Assuming this is the OptionSet value for "First Class"
            C = 300, // Assuming this is the OptionSet value for "Second Class"
            D = 400, // Assuming this is the OptionSet value for "Pass"
            F = 500 // Assuming this is the OptionSet value for "Fail"
        }
    }
}
