using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

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
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<EventCalendar> EventCalendars { get; set; }
        public DbSet<Employee> Employees { get; set; }


        public DbSet<ConductReport> ConductReports { get; set; }
        public DbSet<IEP> IEPs { get; set; }
        public DbSet<AssignedClassStudent> AssignedClassStudents { get; set; }

        public DbSet<TicketPurchases> ticketPurchases { get; set; }

        public DbSet<StaffAttendance> StaffAttendances { get; set; }// Sem2 Mobile App 

        
        public DbSet<Class> Classes { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }
        public DbSet<TicketBooking> TicketBookings { get; set; }
        public DbSet<Employee> Employees { get; set; }



        public DbSet<TicketPurchases> ticketPurchases { get; set; }

        public DbSet<StaffAttendance> StaffAttendances { get; set; }// Sem2 Mobile App 

        
        public DbSet<Class> Classes { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }
    }
}
