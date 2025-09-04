# Casbin integration with PG and .NET

## Migrations tips
dotnet ef migrations add "InitScissors" --context ScissorsDbContext Scissors/Migrations
dotnet ef migrations add "InitAuthentication" --context AuthenticationDbContext --output-dir Infrastructure/Authentication/Migrations