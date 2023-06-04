using Composer.Shared;

namespace Composer.Server.Services;

public interface ILanguageService
{
    Task<string> GetProjectNameAsync(string projectDescription, string clientName);
    Task<List<RoleDescription>> GetRoleDescriptionsAsync(string projectDescription);
}
