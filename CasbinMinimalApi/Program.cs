using CasbinMinimalApi.Endpoints;
using CasbinMinimalApi.Startup;
using CasbinMinimanApi.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AnalyseConfiguration();
builder.Configuration.ValidateEnvironment();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.ConfigureDatabase();
builder.ConfigureSecurity();
builder.ConfigureCasbin();
builder.Services.ConfigureApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseCasbinAuthorization();
app.MapApiEndpoints();

await app.MigrateAsync();
await app.RunAsync();