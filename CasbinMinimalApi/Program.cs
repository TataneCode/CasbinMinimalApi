using Casbin.Persist.Adapter.EFCore;
using CasbinMinimalApi.Casbin;
using CasbinMinimalApi.Domain;
using CasbinMinimalApi.Endpoints;
using CasbinMinimalApi.Infrastructure;
using CasbinMinimalApi.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

ConfigureDatabase(builder);
ConfigureSecurity(builder);
builder.ConfigureCasbin();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapApiEndpoints();
await MigrateAsync(app);

await app.RunAsync();
return;

void ConfigureDatabase(WebApplicationBuilder webApplicationBuilder)
{
    var connectionString = webApplicationBuilder.Configuration["PG_CONNECTION_STRING"];
    webApplicationBuilder.Services.AddDbContext<ScissorsDbContext>((_, b) =>
    {
        b.UseNpgsql(connectionString, options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory", "scissors");
        });
    });
    webApplicationBuilder.Services.AddScoped<ScissorsDbContext>();
    webApplicationBuilder.Services.AddDbContext<AuthenticationDbContext>((_, b) =>
    {
        b.UseNpgsql(connectionString, options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory", "authentication");
        });
    });
    webApplicationBuilder.Services.AddScoped<AuthenticationDbContext>();
}

void ConfigureSecurity(WebApplicationBuilder builder1)
{
    builder1.Services.AddIdentityCore<NeighborUser>(options =>
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
    builder1.Services
        .AddAuthentication(opts => opts.DefaultScheme = IdentityConstants.ApplicationScheme)
        .AddIdentityCookies();

    builder1.Services.AddAuthorization();
}

async Task MigrateAsync(WebApplication webApplication)
{
    using var scope = webApplication.Services.CreateScope();
    var scissorsDbContext = scope.ServiceProvider.GetRequiredService<ScissorsDbContext>();
    await scissorsDbContext.Database.MigrateAsync();
    var authDbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
    await authDbContext.Database.MigrateAsync();
    var authorizationService = scope.ServiceProvider.GetRequiredService<IInitializationService>();
    await authorizationService.LoadPoliciesAsync();
}