namespace SchoolManagmentSystem.Models
{
    public class CreateActivityRequest
    {
        public int TeacherID { get; set; }

        public string HomeworkDescription { get; set; }

        public List<int> StudentIDs { get; set; }
    }
}
