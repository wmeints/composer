using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using Composer.Server.Models;
using Composer.Shared;

namespace Composer.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {

    }

    public DbSet<Proposal> Proposals => Set<Proposal>();
    public DbSet<RoleDescription> RoleDescriptions => Set<RoleDescription>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Proposal>().Property<byte[]>("RowVersion").IsRowVersion();
        builder.Entity<Proposal>().HasMany(x => x.Roles).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.Entity<RoleDescription>().Property<byte[]>("RowVersion").IsRowVersion();
    }
}
