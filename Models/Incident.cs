using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class Incident
    {
       
            public int IncidentId { get; set; }
            
            public int TeacherId { get; set; }
            public string? TeacherName { get; set; }
            public string? StudentName { get; set; }
            public DateTime Date { get; set; }
            public string? Description { get; set; }
            public string? ActionTaken { get; set; }
        
    }
}
