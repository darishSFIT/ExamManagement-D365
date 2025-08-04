namespace proMX.ExamManagement.Plugins.Wellknown
{
    internal class Exam
    {
        public const string LogicalName = "dc_exam";
        public const string ID = "dc_examid"; // GUID
        public const string CourseName = "dc_courseid"; // Lookup to Course
        public const string DurationHours = "dc_durationhours"; // Duration in hours
        public const string ExamDate = "dc_examdate"; // date and time of the exam
        public const string ExamId = "dc_examidnew"; // Autonumber
        public const string ExamName = "dc_examname"; // name of the exam, e.g. "IOT Exam 2023"
        public const string ScheduleName = "dc_schedulename"; // Lookup to Schedule
        public const string ScheduleType = "dc_scheduletype";  // Lookup, but should be OptionSet
        public const string TotalMarks = "dc_totalmarks"; // Total marks for the exam
    }
}