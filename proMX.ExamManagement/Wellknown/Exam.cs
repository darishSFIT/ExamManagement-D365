namespace proMX.ExamManagement.Plugins.Wellknown
{
    internal class Exam
    {
        public const string LogicalName = "dc_exam";
        public const string ID = "dc_examid";
        public const string CourseName = "dc_courseid"; // Lookup to Course
        public const string DurationHours = "dc_durationhours";
        public const string ExamDate = "dc_examdate";
        public const string ExamId = "dc_examidnew"; // Autonumber
        public const string ExamName = "dc_examname";
        public const string ScheduleName = "dc_schedulename"; // Lookup to Schedule
        public const string ScheduleType = "dc_scheduletype";  // Lookup, but should be OptionSet
        public const string TotalMarks = "dc_totalmarks";
    }
}