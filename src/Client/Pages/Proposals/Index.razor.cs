

namespace Composer.Client.Pages.Proposals;

public partial class Index
{
    public bool IsLoading { get; set; } = true;
    public IEnumerable<ProposalSummary> Proposals { get; set; } = new List<ProposalSummary>();
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await ProposalClient.GetProposalsAsync(PageIndex, 20);
        
        PageIndex = response.PageIndex;
        TotalPages = (int)Math.Ceiling((double)response.TotalItems / response.PageSize);
        Proposals = response.Items;

        IsLoading = false;
    }

    public async Task SelectPage(int pageIndex)
    {
        IsLoading = true;

        var response = await ProposalClient.GetProposalsAsync(pageIndex, 20);

        PageIndex = response.PageIndex;
        TotalPages = (int)Math.Ceiling((double)response.TotalItems / response.PageSize);
        Proposals = response.Items;

        IsLoading = false;
    }

    public void EditProposal(long proposalId)
    {
        Navigation.NavigateTo($"/proposals/{proposalId}");
    }
}
