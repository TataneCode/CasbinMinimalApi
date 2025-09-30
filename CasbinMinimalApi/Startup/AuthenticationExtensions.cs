using CasbinMinimalApi.Constants;
using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Persistence.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CasbinMinimalApi.Startup;

public static class AuthenticationExtensions
{
    public static void ConfigureSecurity(this WebApplicationBuilder builder)
    {
        var openIdEnabled = Environment.GetEnvironmentVariable(ConfigurationKey.OpenIdEnabled) == OpenIdStatus.Enabled;

        builder.Services
            .AddIdentityCore<NeighborUser>(o => o.User.RequireUniqueEmail = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddApiEndpoints();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                if (openIdEnabled)
                    options.DefaultChallengeScheme = "zitadel";
            })
            .AddIdentityCookies();

        if (openIdEnabled)
        {
            var configuration = builder.Configuration;
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

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Name = "casbin-minimal-api-auth";
            options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        });

        builder.Services.AddAuthorization();
    }
}