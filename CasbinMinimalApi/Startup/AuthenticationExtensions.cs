using CasbinMinimalApi.Constants;
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
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.Name = "casbin-minimal-api-auth";
        });
        
        // Add OpenID Connect authentication
        var configuration = builder.Configuration;
        if (Environment.GetEnvironmentVariable(ConfigurationKey.OpenIdEnabled) ==  OpenIdStatus.Enabled)
        {
            builder.Services.AddAuthentication()
                .AddOpenIdConnect("zitadel", options =>
                {
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.Authority = configuration.GetValue<string>(ConfigurationKey.OpenIdAuthority);
                    options.ClientId = configuration.GetValue<string>(ConfigurationKey.OpenIdClientId);
                    options.ClientSecret = configuration.GetValue<string>(ConfigurationKey.OpenIdClientSecret);
                    options.ResponseType = "code";
                    options.SaveTokens = true;
                    options.TokenValidationParameters.NameClaimType = "preferred_username";
                    options.TokenValidationParameters.RoleClaimType = "roles";
                    options.RequireHttpsMetadata = false;
                });
        }

        builder.Services.AddAuthorization();
    }
}