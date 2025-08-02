namespace proMX.ExamManagement.Wellknown
{
    internal class Instructor
    {
        public const string LogicalName = "dc_instructor";
        public const string Id = "dc_instructorid"; //GUID
        public const string InstructorName = "dc_instructorname"; //pnc
        public const string InstructorID = "dc_instructoridnum"; //autonumber
        public const string Email = "dc_email";
        public const string Phone = "dc_phone";
        public const string CourseName = "dc_coursename"; // Lookup to Course
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
