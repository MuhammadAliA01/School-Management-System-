namespace SchoolManagmentSystem.Models
{
    public class MedsAdministeredRequest
    {
        public int studentID { get; set; }

        public int teacherID { get; set; }

        public int medID { get; set; } //what the teacher picks from the list

        public DateTime dateAdministered { get; set; }
    }
}
