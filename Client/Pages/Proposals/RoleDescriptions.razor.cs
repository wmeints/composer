using Microsoft.AspNetCore.Components.Web;
using System.Runtime.InteropServices;

namespace Composer.Client.Pages.Proposals;

public partial class RoleDescriptions
{
    [Parameter]
    public string? ProposalId { get; set; } = null!;

    public bool IsLoading { get; set; } = true;

    public List<RoleDescription> Roles { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        Roles = await ProposalClient.GetRolesAsync(Int64.Parse(ProposalId ?? "0"));
        IsLoading = false;
    }

    public async Task GenerateRoleDescriptions(MouseEventArgs e)
    {
        IsLoading = true;
        Roles = await ProposalClient.GenerateRolesAsync(Int64.Parse(ProposalId ?? "0"));
        IsLoading = false;
    }

    private string GenerateDetailsLink(string item)
    {
        return $"/proposals/{ProposalId}/{item}";
    }
}
