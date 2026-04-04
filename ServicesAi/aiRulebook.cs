namespace SchoolManagmentSystem.ServicesAi
{
    public interface aiRulebook
    {  
       Task<string> GetAdvice(string level, string type, string parentComments, string medicalText);
    }
}
