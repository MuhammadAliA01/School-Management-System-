using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class AttendanceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int attendanceID { get; set; }

        [Required]
        public int studentID { get; set; }

        [ForeignKey("studentID")]
        public virtual Student Student {get;set;}

        [Required]
        public int teacherID { get; set; }

        [ForeignKey("teacherID")]
        public virtual Staff Staff { get; set; }
        public DateTime DateAttended { get; set; }
        public string status { get; set; }
    }
}
