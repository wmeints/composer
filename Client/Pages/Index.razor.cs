using Composer.Client.Services;
using System.Net;

namespace Composer.Client.Pages
{
    public partial class Index
    {
        public InputModel Input { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; } = null;

        public async Task StartProposal()
        {
            IsLoading = true;

            try
            {
                var response = await ProposalClient.StartProposal(new Composer.Shared.StartProposalRequest
                {
                    ClientName = Input.ClientName,
                    Description = Input.Description,
                });

                NavigationManager.NavigateTo($"/proposals/{response.Id}");
            }
            catch (WebException)
            {
                IsLoading = false;
                ErrorMessage = "Failed to start proposal. Please try again later.";
            }
        }

        public void RedirecTtoSignIn()
        {
            NavigationManager.NavigateToLogin("authentication/login");
        }

        public class InputModel
        {
            public string ClientName { get; set; } = "";
            public string Description { get; set; } = "";
        }
    }
}
