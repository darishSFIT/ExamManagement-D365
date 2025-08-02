namespace proMX.ExamManagement.Plugins.Wellknown
{
    internal class Course
    {
        public const string LogicalName = "dc_course";
        public const string Id = "dc_courseid";  //GUID
        public const string Name = "dc_coursename";
        public const string CourseID = "dc_courseidnum"; // autonumber
        public const string Description = "dc_description";
        public const string Duration = "dc_duration";
        public const string Credits = "dc_credits";
        public const string InstructorName = "dc_instructorname";
        public const string Department = "dc_departmentoptions";

        public enum Course_Department
        {
            INFT = 121750000,
            CMPN = 121750001,
            MECH = 121750002,
            EXTC = 121750003,
            ELEC = 121750004,
            AIML = 121750005
        }
    }
}