# Authentication & Authorization Minimal API with Casbin

## Project description
This project is a Minimal API example with authentication and authorization using Casbin.
It includes :
- .net 10 (preview)
- Default .net Identity authentication
- Casbin with EFCore adapter
- Postgres database
- Docker compose

## Api description
As .net 10 default behaviour, once started, the description of the API is available here :
- http://localhost:8680/openapi/v1.json

## Migrations tips
As there are no default dbcontext factory, follow those steps to run migration :
- Place yourself in the CasbinMinimalApi project folder
- Set env variable before generating
  - Connection string : Can be anything
  - example : $env:PG_CONNECTION_STRING="_"
- Run the migration according the context
  - dotnet ef migrations add "\<MigrationName\>" --context ScissorsDbContext --output-dir Persistence/Scissors/Migrations
  - dotnet ef migrations add "\<MigrationName\>" --context AuthenticationDbContext --output-dir Persistence/Authentication/Migrations