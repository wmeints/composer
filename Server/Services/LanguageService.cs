using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace Composer.Server.Services;

public class LanguageService : ILanguageService
{
    private readonly LanguageServiceOptions _languageServiceOptions;

    public LanguageService(IOptions<LanguageServiceOptions> options)
    {
        _languageServiceOptions = options.Value;
    }

    public async Task<string> GetProjectNameAsync(string projectDescription, string clientName)
    {
        var kernel = Kernel.Builder.Build();

        kernel.Config.AddOpenAITextCompletionService(_languageServiceOptions.ModelName, _languageServiceOptions.ApiKey);

        var skills = kernel.ImportSemanticSkillFromDirectory("Skills", "ProjectName");

        var input = new ContextVariables
        {
            ["description"] = projectDescription,
            ["clientName"] = clientName
        };

        var output = await kernel.RunAsync(input, skills["Name"]);

        return output.Result;
    }
}
