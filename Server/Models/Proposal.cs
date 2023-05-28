namespace Composer.Server.Models;

public class Proposal
{
    public long Id { get; set; }
    public string ClientName { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
