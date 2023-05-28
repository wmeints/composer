using Composer.Server.Data;
using Composer.Server.Models;
using Composer.Server.Services;
using Composer.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Composer.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/proposals")]
public class ProposalsController : ControllerBase
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILanguageService _languageService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProposalsController(ApplicationDbContext applicationDbContext, ILanguageService languageService, UserManager<ApplicationUser> userManager)
    {
        _applicationDbContext = applicationDbContext;
        _languageService = languageService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<GetProposalsResponse> GetProposals(int pageIndex, int pageSize)
    {
        var totalItems = await _applicationDbContext.Proposals.LongCountAsync();

        var records = await _applicationDbContext.Proposals
            .OrderByDescending(x => x.CreatedAt)
            .Where(x => x.Author.Id == _userManager.GetUserId(User))
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(x => new ProposalSummary
            {
                Id = x.Id,
                ClientName = x.ClientName,
                ProjectName = x.ProjectName,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return new GetProposalsResponse
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalItems = totalItems,
            Items = records
        };
    }

    [HttpGet("{proposalId}")]
    public async Task<IActionResult> GetProposal(long proposalId)
    {
        var proposal = await _applicationDbContext.Proposals
            .Where(x => x.Author.Id == _userManager.GetUserId(User))
            .FirstOrDefaultAsync(x => x.Id == proposalId);

        if (proposal == null)
        {
            return NotFound();
        }

        return Ok(proposal);
    }

    [HttpPost]
    public async Task<IActionResult> StartProposal([FromBody] StartProposalRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var projectName = await _languageService.GetProjectNameAsync(request.Description, request.ClientName);

        var proposal = new Proposal
        {
            ClientName = request.ClientName,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            ProjectName = projectName,
            Author = user
        };

        _applicationDbContext.Proposals.Add(proposal);
        await _applicationDbContext.SaveChangesAsync();

        return Ok(new StartProposalResponse { Id = proposal.Id });
    }

    public async Task<IActionResult> UpdateProposalDetails(long proposalId, [FromBody] UpdateProposalRequest request)
    {
        var proposal = await _applicationDbContext.Proposals
            .Where(x => x.Author.Id == _userManager.GetUserId(User))
            .FirstOrDefaultAsync(x => x.Id == proposalId);

        if (proposal == null)
        {
            return NotFound();
        }

        proposal.ClientName = request.ClientName;
        proposal.ProjectName = request.ProjectName;
        proposal.Description = request.Description;
        proposal.ModifiedAt = DateTime.UtcNow;

        return Accepted();
    }

    [HttpPost("{proposalId}/project-name")]
    public async Task<IActionResult> GenerateProjectName(long proposalId, [FromBody] GenerateProjectNameRequest request)
    {
        var proposal = await _applicationDbContext.Proposals
            .Where(x => x.Author.Id == _userManager.GetUserId(User))
            .FirstOrDefaultAsync(x => x.Id == proposalId);

        if (proposal == null)
        {
            return NotFound();
        }

        var projectName = await _languageService.GetProjectNameAsync(proposal.Description, proposal.ClientName);

        proposal.ProjectName = projectName;
        proposal.ClientName = request.ClientName;
        proposal.Description = request.Description;

        await _applicationDbContext.SaveChangesAsync();

        return Ok(new GenerateProjectNameResponse
        {
            ProjectName = projectName
        });
    }

    [HttpPost("{proposalId}/roles")]
    public async Task<IActionResult> GenerateRoles(long proposalId)
    {
        var proposal = await _applicationDbContext.Proposals
            .Where(x => x.Author.Id == _userManager.GetUserId(User))
            .FirstOrDefaultAsync(x => x.Id == proposalId);

        if (proposal == null)
        {
            return NotFound();
        }

        var roleDescriptions = await _languageService.GetRoleDescriptionsAsync(proposal.Description);

        proposal.Roles = roleDescriptions;

        await _applicationDbContext.SaveChangesAsync();

        return Ok(roleDescriptions);
    }

    [HttpGet("{proposalId}/roles")]
    public async Task<IActionResult> GetRoles(long proposalId)
    {
        var proposal = await _applicationDbContext.Proposals
            .Include(x => x.Roles)
            .Where(x => x.Author.Id == _userManager.GetUserId(User))
            .FirstOrDefaultAsync(x => x.Id == proposalId);

        if (proposal == null)
        {
            return NotFound();
        }

        return Ok(proposal.Roles);
    }
}
