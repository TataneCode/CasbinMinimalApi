using CasbinMinimalApi.Application.Authorization;
using CasbinMinimalApi.Constants;
using CasbinMinimalApi.Persistence.Authentication;
using CasbinMinimanApi.Persistence.Scissors;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Startup;


public static class DatabaseExtensions
{
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var connectionString =
            builder.Configuration[ConfigurationKey.ConnectionString]
            ?? throw new InvalidOperationException("PG_CONNECTION_STRING is not set");
        builder.Services.AddDbContext<ScissorsDbContext>((_, b) =>
        {
            b.UseNpgsql(connectionString,
                options => { options.MigrationsHistoryTable("__EFMigrationsHistory", "scissors"); });
        });
        builder.Services.AddScoped<ScissorsDbContext>();
        builder.Services.AddDbContext<AuthenticationDbContext>((_, b) =>
        {
            b.UseNpgsql(connectionString,
                options => { options.MigrationsHistoryTable("__EFMigrationsHistory", "authentication"); });
        });
        builder.Services.AddScoped<AuthenticationDbContext>();
    }

    public static async Task MigrateAsync(this WebApplication webApplication)
    {
        if (EF.IsDesignTime) return;

        using var scope = webApplication.Services.CreateScope();

        var scissorsDbContext = scope.ServiceProvider.GetRequiredService<ScissorsDbContext>();
        await scissorsDbContext.Database.MigrateAsync();
        await scissorsDbContext.SeedDataAsync();

        var authDbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
        await authDbContext.Database.MigrateAsync();

        var authorizationService = scope.ServiceProvider.GetRequiredService<IRoleService>();
        if (await authorizationService.HasAnyPoliciesAsync()) return;
        await authorizationService.LoadPoliciesAsync();
    }
}