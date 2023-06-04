using Pulumi;
using Pulumi.AzureNative.Resources;
using SearchSkuName = Pulumi.AzureNative.Search.SkuName;
using SearchSkuArgs = Pulumi.AzureNative.Search.Inputs.SkuArgs;
using SearchService = Pulumi.AzureNative.Search.Service;
using SearchServiceArgs = Pulumi.AzureNative.Search.ServiceArgs;
using ListAdminKey = Pulumi.AzureNative.Search.ListAdminKey;
using ListAdminKeyInvokeArgs = Pulumi.AzureNative.Search.ListAdminKeyInvokeArgs;
using CognitiveService = Pulumi.AzureNative.CognitiveServices.Account;
using CognitiveServiceArgs = Pulumi.AzureNative.CognitiveServices.AccountArgs;
using CognitiveServicesSkuArgs = Pulumi.AzureNative.CognitiveServices.Inputs.SkuArgs;
using ListAccountKeys = Pulumi.AzureNative.CognitiveServices.ListAccountKeys;
using ListAccountKeysInvokeArgs = Pulumi.AzureNative.CognitiveServices.ListAccountKeysInvokeArgs;
using AppServicePlan = Pulumi.AzureNative.Web.AppServicePlan;
using AppServicePlanArgs = Pulumi.AzureNative.Web.AppServicePlanArgs;
using AppServiceSkuArgs = Pulumi.AzureNative.Web.Inputs.SkuDescriptionArgs;
using WebApp = Pulumi.AzureNative.Web.WebApp;
using WebAppArgs = Pulumi.AzureNative.Web.WebAppArgs;
using SiteConfigArgs = Pulumi.AzureNative.Web.Inputs.SiteConfigArgs;
using NameValuePairArgs = Pulumi.AzureNative.Web.Inputs.NameValuePairArgs;
using Deployment = Pulumi.AzureNative.CognitiveServices.Deployment;
using DeploymentPropertiesArgs = Pulumi.AzureNative.CognitiveServices.Inputs.DeploymentPropertiesArgs;
using DeploymentModelArgs = Pulumi.AzureNative.CognitiveServices.Inputs.DeploymentModelArgs;
using DeploymentArgs = Pulumi.AzureNative.CognitiveServices.DeploymentArgs;
using System.Collections.Generic;

namespace Composer.Infrastructure;

public class ApplicationInfraLayer
{
    public ApplicationInfraLayer(string location, InputMap<string> tags, Output<string> storageAccountName, Output<string> storageAccountKey)
    {
        var appInfraResourceGroup = new ResourceGroup("composer-app-infrastructure", new ResourceGroupArgs
        {
            Location = location,
            Tags = tags
        });

        var searchEngine = new SearchService("composer-search", new SearchServiceArgs
        {
            PartitionCount = 1,
            Sku = new SearchSkuArgs
            {
                Name = SearchSkuName.Basic
            },
            Location = location,
            ResourceGroupName = appInfraResourceGroup.Name,
            Tags = tags,
            ReplicaCount = 1
        });

        var openAiService = new CognitiveService("composer-openai", new CognitiveServiceArgs
        {
            Kind = "OpenAI",
            Location = location,
            ResourceGroupName = appInfraResourceGroup.Name,
            Tags = tags,
            Sku = new CognitiveServicesSkuArgs
            {
                Name = "S0"
            }
        });

        var modelDeployment = new Deployment("davinci", new DeploymentArgs
        {
            AccountName = openAiService.Name,
            ResourceGroupName = appInfraResourceGroup.Name,
            Properties = new DeploymentPropertiesArgs
            {
                Model = new DeploymentModelArgs
                {
                    Name = "text-davinci-003",
                    Version = "1",
                    Format = "OpenAI"
                },
                ScaleSettings = new Pulumi.AzureNative.CognitiveServices.Inputs.DeploymentScaleSettingsArgs
                {
                    ScaleType = "Standard"
                }
            }
        });

        var listAdminKeys = ListAdminKey.Invoke(new ListAdminKeyInvokeArgs
        {
            ResourceGroupName = appInfraResourceGroup.Name,
            SearchServiceName = searchEngine.Name
        });

        var searchServiceKey = listAdminKeys.Apply(keys => keys.PrimaryKey);

        var listKeys = ListAccountKeys.Invoke(new ListAccountKeysInvokeArgs
        {
            AccountName = openAiService.Name,
            ResourceGroupName = appInfraResourceGroup.Name
        });

        var languageModelKey = listKeys.Apply(keys => keys.Key1!);

        var servicePlan = new AppServicePlan("composer-asp", new AppServicePlanArgs
        {
            Tags = tags,
            ResourceGroupName = appInfraResourceGroup.Name,
            Location = location,
            Sku = new AppServiceSkuArgs
            {
                Tier = "Basic",
                Name = "B1",
            }
        });

        var webApp = new WebApp("composer-app", new WebAppArgs
        {
            ServerFarmId = servicePlan.Id,
            ResourceGroupName = appInfraResourceGroup.Name,
            Tags = tags,
            SiteConfig = new SiteConfigArgs
            {
                AppSettings = new List<NameValuePairArgs>
                {
                    new NameValuePairArgs
                    {
                        Name = "LanguageService:Endpoint",
                        Value = $"https://{openAiService.Name}.openai.azure.com"
                    },
                    new NameValuePairArgs
                    {
                        Name = "LanguageService:ApiKey",
                        Value = languageModelKey
                    },
                    new NameValuePairArgs
                    {
                        Name = "LanguageService:ModelName",
                        Value = "davinci"
                    },
                    new NameValuePairArgs
                    {
                        Name = "Search:Endpoint",
                        Value = $"https://{searchEngine.Name}.search.windows.net"
                    },
                    new NameValuePairArgs
                    {
                        Name = "Search:ApiKey",
                        Value = searchServiceKey
                    },
                    new NameValuePairArgs
                    {
                        Name = "Storage:AccountName",
                        Value = storageAccountName
                    },
                    new NameValuePairArgs
                    {
                        Name = "Storage:AccountKey",
                        Value = storageAccountKey
                    }
                }
            }
        });

        WebAppUrl = webApp.DefaultHostName;
    }

    public Output<string> WebAppUrl { get; set; }
}
