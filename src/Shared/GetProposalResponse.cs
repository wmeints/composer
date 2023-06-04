namespace Composer.Shared;

public class GetProposalResponse
{
    public long Id { get; set; }
    public string ClientName { get; set; } = "";
    public string Description { get; set; } = "";
    public string ProjectName { get; set; } = "";
}
