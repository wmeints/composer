namespace Composer.Server.Services;

public interface ILanguageService
{
    Task<string> GetProjectNameAsync(string projectDescription, string clientName);
}
