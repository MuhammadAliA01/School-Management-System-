using System;
using System.Collections.Generic;

namespace SchoolManagmentSystem.Models
{
    public class AssignScheduleViewModel
    {
        public List<Staff> AvailableStaff { get; set; } = new List<Staff>();

        public List<Class> UnassignedClasses { get; set; } = new List<Class>();

        public List<StaffSchedule> Schedules { get; set; } = new List<StaffSchedule>();

        public DateTime TargetScheduleDate { get; set; }
    }
}