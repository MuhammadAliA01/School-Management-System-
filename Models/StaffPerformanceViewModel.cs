namespace SchoolManagmentSystem.Models
{
    public class StaffPerformanceViewModel
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public double AverageScore { get; set; }
        public int WeeksLogged { get; set; }
        public int TotalAssessments { get; set; }
    }
}