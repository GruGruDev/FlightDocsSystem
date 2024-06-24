namespace FlightDocsSystem.Services.Auth.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string fullName, string to, string subject, string body);
    }
}
