namespace Composer.Shared;

public class GetProposalsResponse
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public long TotalItems { get; set; }
    public IEnumerable<ProposalSummary> Items { get; set; } = null!;
}
