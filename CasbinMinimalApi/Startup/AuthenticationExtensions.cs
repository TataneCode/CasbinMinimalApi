using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Persistence.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CasbinMinimalApi.Startup;

public static class AuthenticationExtensions
{
    public static void ConfigureSecurity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentityCore<NeighborUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddApiEndpoints();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            })
            .AddBearerToken(IdentityConstants.BearerScheme)
            .AddIdentityCookies();

        builder.Services.AddAuthorization();
    }
}