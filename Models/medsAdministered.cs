using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagmentSystem.Models
{
    public class medsAdministered
    {

        [Key]
        public int ID { get; set; }

        public int studentID { get; set; }

        [ForeignKey("studentID")]
        public virtual Student student { get; set; }

        public int teacherID { get; set; }

        [ForeignKey("teacherID")]
        public virtual Staff staff { get; set; }

        public int medID { get; set; }

        [ForeignKey("medID")]
        public virtual Medication medication { get; set; }

        public DateTime dateAdministered { get; set; }
    }
}
