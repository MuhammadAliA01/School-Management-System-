namespace SchoolManagmentSystem.Services
{
    public interface IApplicationService
    {
        Task<string> ProcessApplicationAsync(IFormFile idDoc, IFormFile appForm);

    }
}
