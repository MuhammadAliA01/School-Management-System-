using System;
using System.Collections.Generic;

namespace SchoolManagmentSystem.Models
{
    public class StudentRow
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    //  This is the "digital cardboard box" that holds the whole screen's data
    public class MorningAttendanceData
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public List<StudentRow> Students { get; set; } = new List<StudentRow>();
    }
}