using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Startup;

public static class FailfastExtensions
{
  public static void ValidateEnvironment(this IConfiguration configuration)
  {
    string[] requiredSettings = ["PG_CONNECTION_STRING"];
    if (!EF.IsDesignTime)
    {
      requiredSettings = [.. requiredSettings, "PGUSER", "PGPASSWORD"];
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