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

        public DbSet<ActivityModel> ActivityModels { get; set; }
        public DbSet<ConductReport> ConductReports { get; set; }

        public DbSet<StudentPickup> StudentPickups { get; set; }

        public DbSet<IEP> IEPs { get; set; }
        public DbSet<AssignedClassStudent> AssignedClassStudents { get; set; }

        public DbSet<TicketPurchases> ticketPurchases { get; set; }

        public DbSet<StaffAttendance> StaffAttendances { get; set; }// Sem2 Mobile App 

        
        public DbSet<Class> Classes { get; set; }
        public DbSet<StaffSchedule> StaffSchedules { get; set; }
        public DbSet<TicketBooking> TicketBookings { get; set; }

        public DbSet<AttendanceModel> AttendanceModels { get; set; }

        public DbSet<medsAdministered> medsAdministered { get; set; }
        public DbSet<Medication> Medications { get; set; }

        // Payslip and PayslipVersionControl DbSets
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<PayslipVersionControl> PayslipVersionControls { get; set; }

        public DbSet<EmployeeEarnings> EmployeeEarnings { get; set; }
        //  TAX FEATURE TABLES
        public DbSet<TaxBracket> TaxBrackets { get; set; }
        public DbSet<TaxConfiguration> TaxConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  SARS Tax Brackets
            modelBuilder.Entity<TaxBracket>().HasData(
                new TaxBracket { Id = 1, TaxYear = "2026/2027", MinIncome = 1, MaxIncome = 237100, BaseTaxAmount = 0, TaxRate = 0.18 },
                new TaxBracket { Id = 2, TaxYear = "2026/2027", MinIncome = 237101, MaxIncome = 370500, BaseTaxAmount = 42678, TaxRate = 0.26 },
                new TaxBracket { Id = 3, TaxYear = "2026/2027", MinIncome = 370501, MaxIncome = 512800, BaseTaxAmount = 77362, TaxRate = 0.31 },
                new TaxBracket { Id = 4, TaxYear = "2026/2027", MinIncome = 512801, MaxIncome = 673000, BaseTaxAmount = 121475, TaxRate = 0.36 },
                new TaxBracket { Id = 5, TaxYear = "2026/2027", MinIncome = 673001, MaxIncome = 857900, BaseTaxAmount = 179147, TaxRate = 0.39 },
                new TaxBracket { Id = 6, TaxYear = "2026/2027", MinIncome = 857901, MaxIncome = 1817000, BaseTaxAmount = 251258, TaxRate = 0.41 },
                new TaxBracket { Id = 7, TaxYear = "2026/2027", MinIncome = 1817001, MaxIncome = 99999999, BaseTaxAmount = 644489, TaxRate = 0.45 }
            );

            //  SARS Rebates & Contribution Rules
            modelBuilder.Entity<TaxConfiguration>().HasData(
                new TaxConfiguration
                {
                    Id = 1,
                    TaxYear = "2026/2027",
                    PrimaryRebate = 17235.0,
                    SecondaryRebate = 9444.0,
                    TertiaryRebate = 3145.0,
                    PensionContributionRate = 0.075,
                    UifRate = 0.01,
                    UifMonthlyCap = 177.12
                }
            );
        }

    }
}
