namespace Composer.Client.Pages.Proposals;

public partial class Details
{
    [Parameter]
    public string? ProposalId { get; set; }

    public bool IsLoading { get; set; } = true;

    public InputModel Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var response = await ProposalClient.GetProposalAsync(long.Parse(ProposalId ?? "0"));

        if (response == null)
        {
            return;
        }

        Input.ProjectName = response.ProjectName;
        Input.ClientName = response.ClientName;
        Input.Description = response.Description;

        IsLoading = false;
    }

    public async Task UpdateProjectDetails()
    {

    }

    public async Task GenerateProjectName()
    {
        var result = await ProposalClient.GenerateProjectNameAsync(long.Parse(ProposalId ?? "0"), new Composer.Shared.GenerateProjectNameRequest
        {
            ClientName = Input.ClientName,
            Description = Input.Description
        });

        Input.ProjectName = result.ProjectName;
    }

    public async Task SaveChanges()
    {
        await ProposalClient.UpdateProposalAsync(Int64.Parse(ProposalId ?? "0"), new Composer.Shared.UpdateProposalRequest
        {
            ClientName = Input.ClientName,
            Description = Input.Description,
            ProjectName = Input.ProjectName
        });
    }

    public class InputModel
    {
        public string ClientName { get; set; } = "";
        public string ProjectName { get; set; } = "";
        public string Description { get; set; } = "";
    }
}
