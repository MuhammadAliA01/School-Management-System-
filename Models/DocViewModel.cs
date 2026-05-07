namespace SchoolManagmentSystem.Models
{
    public class DocViewModel
    {
        public IFormFile IDDocument { get; set; }

        public IFormFile ApplicationForm { get; set; }
        public string? ResultMessage
        {
            get; set;
        }
    }
}
