using Microsoft.EntityFrameworkCore;

namespace SchoolManagmentSystem.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Parent> Parents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<EmploymentApplication> EmploymentApplications { get; set; }
        public DbSet<Incident> Incidents { get; set; }

        public DbSet<StudentAssessment> StudentAssessments { get; set; }
        public DbSet<StudentRegistration> StudentRegistrations { get; set; }
        public DbSet<StudentDocument> StudentDocuments { get; set; }   //NOTE: I HAVEN'T UPDATED TO DATABASE
        public DbSet<Attendance>Attendances { get; set; }
        public DbSet<EventCalendar> EventCalendars { get; set; }
        public DbSet<TicketBooking> TicketBookings { get; set; }


    }
}
