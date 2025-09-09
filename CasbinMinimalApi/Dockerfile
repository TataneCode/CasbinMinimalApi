FROM mcr.microsoft.com/dotnet/nightly/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/nightly/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CasbinMinimalApi/CasbinMinimalApi.csproj", "CasbinMinimalApi/"]
RUN dotnet restore "CasbinMinimalApi/CasbinMinimalApi.csproj"
COPY . .
WORKDIR "/src/CasbinMinimalApi"
RUN dotnet build "CasbinMinimalApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CasbinMinimalApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CasbinMinimalApi.dll"]
