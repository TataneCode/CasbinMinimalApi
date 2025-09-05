using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Persistence.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CasbinMinimalApi.Startup;

public static class AuthenticationExtensions
{
    public static void ConfigureSecurity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentityCore<NeighborUser>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
                options.User.RequireUniqueEmail = true;
            })
            .AddSignInManager<SignInManager<NeighborUser>>()
            .AddUserManager<UserManager<NeighborUser>>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();
        builder.Services
            .AddAuthentication(opts => opts.DefaultScheme = IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services.AddAuthorization();
    }
}