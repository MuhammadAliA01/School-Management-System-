using System.Collections.Generic;
using System;

namespace SchoolManagmentSystem.Models
{
    public class AssignScheduleViewModel
    {
        // Lists to display on the dashboard
        public List<Employee> AvailableStaff { get; set; } = new List<Employee>();
        public List<Class> UnassignedClasses { get; set; } = new List<Class>();

        // The date the system will automatically assign (e.g., Tomorrow)
        public DateTime TargetScheduleDate { get; set; }
    }
}