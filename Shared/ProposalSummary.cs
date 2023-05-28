namespace Composer.Shared;

public class ProposalSummary
{
    public long Id { get; set; }
    public string ClientName { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}