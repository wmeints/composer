using Pulumi;
using Pulumi.AzureNative.Resources;
using AccessTier = Pulumi.AzureNative.Storage.AccessTier;
using StorageSkuArgs = Pulumi.AzureNative.Storage.Inputs.SkuArgs;
using StorageSkuName = Pulumi.AzureNative.Storage.SkuName;
using StorageKind = Pulumi.AzureNative.Storage.Kind;
using StorageAccount = Pulumi.AzureNative.Storage.StorageAccount;
using ListStorageAccountKeys = Pulumi.AzureNative.Storage.ListStorageAccountKeys;
using ListStorageAccountKeysInvokeArgs = Pulumi.AzureNative.Storage.ListStorageAccountKeysInvokeArgs;

namespace Composer.Infrastructure;

public class SystemInfraLayer
{
    public SystemInfraLayer(string location, InputMap<string> tags)
    {
        var systemInfraResourceGroup = new ResourceGroup("composer-system-infrastructure", new ResourceGroupArgs
        {
            Location = location,
            Tags = tags
        });

        var storageAccount = new StorageAccount("storcomposerapp", new Pulumi.AzureNative.Storage.StorageAccountArgs
        {
            AccessTier = AccessTier.Hot,
            Sku = new StorageSkuArgs
            {
                Name = StorageSkuName.Standard_LRS
            },
            Kind = StorageKind.StorageV2,
            IsHnsEnabled = true,
            Location = location,
            ResourceGroupName = systemInfraResourceGroup.Name,
        });

        var listStorageKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
        {
            AccountName = storageAccount.Name,
            ResourceGroupName = systemInfraResourceGroup.Name,
        });

        StorageAccountName = storageAccount.Name;
        StorageAccountKey = listStorageKeys.Apply(keys => keys.Keys[0].Value);
    }

    public Output<string> StorageAccountName { get; set; }
    public Output<string> StorageAccountKey { get; set; }
}
