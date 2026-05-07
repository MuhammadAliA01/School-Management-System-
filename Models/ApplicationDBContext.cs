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
        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<EmploymentApplication> EmploymentApplications { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<StudentRegistration> StudentRegistrations { get; set; }
        public DbSet<ConductReport> ConductReports { get; set; }
        public DbSet<IEP> IEPs { get; set; }
    }
}
