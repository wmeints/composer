using Composer.Shared;
using System.Net;
using System.Net.Http.Json;

namespace Composer.Client.Services;

public class ProposalClient
{
    private readonly HttpClient _httpClient;

    public ProposalClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StartProposalResponse> StartProposal(StartProposalRequest request)
    {
        var response  = await _httpClient.PostAsJsonAsync("/api/proposals", request);
        return await response.Content.ReadFromJsonAsync<StartProposalResponse>();
    }

    public async Task<GetProposalResponse?> GetProposalAsync(long proposalId)
    {
        var response = await _httpClient.GetAsync($"/api/proposals/{proposalId}");

        if(response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<GetProposalResponse>();
    }

    public async Task<GetProposalsResponse> GetProposalsAsync(int pageIndex, int pageSize)
    {
        var response = await _httpClient.GetAsync($"/api/proposals?pageIndex={pageIndex}&pageSize={pageSize}");
        return await response.Content.ReadFromJsonAsync<GetProposalsResponse>();
    }

    public async Task<GenerateProjectNameResponse> GenerateProjectNameAsync(long proposalId, GenerateProjectNameRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/proposals/{proposalId}/project-name", request);

        if(response.StatusCode == HttpStatusCode.BadRequest)
        {
            // TODO: Parse the error message and return it.
        }

        if(response.StatusCode == HttpStatusCode.NotFound)
        {
            //TODO: Return a not-found result.
        }

        return await response.Content.ReadFromJsonAsync<GenerateProjectNameResponse>();
    }

    public async Task UpdateProposalAsync(long proposalId, UpdateProposalRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/proposals/{proposalId}", request);
        response.EnsureSuccessStatusCode();
    }
}
