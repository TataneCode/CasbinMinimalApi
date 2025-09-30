using CasbinMinimalApi.Constants;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Startup;

public static class FailfastExtensions
{
    public static void ValidateEnvironment(this IConfiguration configuration)
    {
        string[] requiredSettings = [ConfigurationKey.ConnectionString];
        if (!EF.IsDesignTime)
        {
            requiredSettings = [.. requiredSettings, ConfigurationKey.DatabaseUser, ConfigurationKey.DatabasePassword];
        }

        if (Environment.GetEnvironmentVariable(ConfigurationKey.OpenIdEnabled) == OpenIdStatus.Enabled)
        {
            requiredSettings =
            [
                ..requiredSettings, ConfigurationKey.OpenIdClientId, ConfigurationKey.OpenIdClientSecret,
                ConfigurationKey.OpenIdAuthority
            ];
        }

        var missing = requiredSettings
            .Where(key => string.IsNullOrWhiteSpace(configuration[key]))
            .ToList();

        if (missing.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing required environment variables: {string.Join(", ", missing)}");
        }
    }
}