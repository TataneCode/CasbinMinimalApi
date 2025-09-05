using CasbinMinimalApi.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Persistence.Authentication;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : IdentityDbContext<NeighborUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("authentication");
        base.OnModelCreating(builder);
    }
}