namespace SchoolManagmentSystem.Models
{
        public class FlaggedStudent
        {
            public int StudentId { get; set; }
            public string Name { get; set; }
            public int TotalAbsences { get; set; }
        }

        public class InsightsOnStudentAttendance
        {
            public int TotalStudents { get; set; }
            public int PresentToday { get; set; }
            public double DailyAttendanceRate { get; set; }

            public List<FlaggedStudent> AtRiskStudents { get; set; } = new List<FlaggedStudent>();
        }
}

