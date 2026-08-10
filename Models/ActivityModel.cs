using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class ActivityModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ActivityID { get; set; }

        public int StudentID { get; set; }

        public int TeacherID { get; set; }

        public string HomeworkDescription { get; set; }
    }
}
