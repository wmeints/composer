using Pulumi;

namespace Composer.Infrastructure;

public class ComposerStack : Stack
{
    public ComposerStack()
    {
        var location = "westeurope";
        var tags = new InputMap<string>
        {
            ["Project"] = "Composer",
            ["Customer"] = "Aigency"
        };

        var systemInfraStructureLayer = new SystemInfraLayer(location, tags);
        var applicationInfraStructureLayer = new ApplicationInfraLayer(
            location, tags, 
            systemInfraStructureLayer.StorageAccountName, 
            systemInfraStructureLayer.StorageAccountKey);
    }

}
