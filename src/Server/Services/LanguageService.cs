﻿using Composer.Shared;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.AzureSdk;
using Microsoft.SemanticKernel.Orchestration;
using System.Text.RegularExpressions;

namespace Composer.Server.Services;

public class LanguageService : ILanguageService
{
    private readonly LanguageServiceOptions _languageServiceOptions;
    private readonly ILogger<LanguageService> _logger;

    private static Regex RoleAndDescriptionPattern = new Regex("^(\\d+). (?<role>.+): (?<description>.+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public LanguageService(IOptions<LanguageServiceOptions> options, ILogger<LanguageService> logger)
    {
        _languageServiceOptions = options.Value;
        _logger = logger;
    }

    public async Task<string> GetProjectNameAsync(string projectDescription, string clientName)
    {
        using var _ = Activities.GetProjectName();

        var kernel = Kernel.Builder.Build();

        kernel.Config.AddAzureTextCompletionService(
            _languageServiceOptions.ModelName, 
            _languageServiceOptions.Endpoint, 
            _languageServiceOptions.ApiKey);

        var skills = kernel.ImportSemanticSkillFromDirectory("Skills", "ProjectName");

        var input = new ContextVariables
        {
            ["description"] = projectDescription,
            ["clientName"] = clientName
        };

        var output = await kernel.RunAsync(input, skills["Name"]);

        return output.Result;
    }

    public async Task<List<RoleDescription>> GetRoleDescriptionsAsync(string projectDescription)
    {
        using var _ = Activities.GetProjectRoles();

        var kernel = Kernel.Builder.Build();

        kernel.Config.AddOpenAITextCompletionService(
            _languageServiceOptions.ModelName, _languageServiceOptions.ApiKey);

        var skills = kernel.ImportSemanticSkillFromDirectory("Skills", "ProjectRoles");

        var input = new ContextVariables
        {
            ["INPUT"] = projectDescription,
        };

        var output = await kernel.RunAsync(input, skills["Summarize"], skills["ProvideRoles"]);

        var roles = output.Result
            .Split("\n")
            .Select(x =>
            {
                var match = RoleAndDescriptionPattern.Match(x);
                return new RoleDescription(0, match.Groups["role"].Value.Trim(), match.Groups["description"].Value.Trim());
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Name) && !string.IsNullOrWhiteSpace(x.Description))
            .ToList();

        return roles;
    }
}
