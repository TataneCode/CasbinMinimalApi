using CasbinMinimalApi.Constants;
using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Persistence.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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

        if (openIdEnabled)
        {
            var configuration = builder.Configuration;
            var environment = builder.Environment;
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = IdentityConstants.ApplicationScheme;
                    options.Authority = configuration.GetValue<string>(ConfigurationKey.OpenIdAuthority);
                    options.ClientId = configuration.GetValue<string>(ConfigurationKey.OpenIdClientId);
                    options.ClientSecret = configuration.GetValue<string>(ConfigurationKey.OpenIdClientSecret);
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.SaveTokens = true;
                    options.ClaimsIssuer = options.Authority;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.TokenValidationParameters.NameClaimType = "preferred_username";
                    //options.RequireHttpsMetadata = false;
                    if (environment.IsDevelopment() &&
                        configuration.GetValue(ConfigurationKey.DisabledTlsValidation, false))
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        };
                    }
                })
                .AddIdentityCookies();
        }
        else
        {
            builder.Services
                .AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();
        }

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.Name = "casbin-minimal-api-auth";
        });

        builder.Services.AddAuthorization();
    }
}