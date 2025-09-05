using CasbinMinimalApi.Application.Repositories;
using CasbinMinimalApi.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CasbinMinimalApi.Endpoints;

public static class StuffEndpoints
{
  public static RouteGroupBuilder MapStuffEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/stuffs")
        .WithTags("Stuffs");

    group.MapGet("/", GetAllAsync);
    group.MapGet("/{id:long}", GetByIdAsync).WithName("GetStuffById");
    group.MapPost("/", CreateAsync).RequireAuthorization();
    group.MapPut("/{id:long}", UpdateAsync).RequireAuthorization();
    group.MapDelete("/{id:long}", DeleteAsync).RequireAuthorization();

    return group;
  }

  private static async Task<Ok<IEnumerable<StuffDto>>> GetAllAsync(IStuffRepository repo)
  {
    var items = await repo.GetAllAsync();
    return TypedResults.Ok(items.Select(s => new StuffDto(s.Id, s.Name, s.Description, s.NeighborId)));
  }

  private static async Task<Results<Ok<StuffDto>, NotFound>> GetByIdAsync(long id, IStuffRepository repo)
  {
    var entity = await repo.GetByIdAsync(id);
    return entity is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(new StuffDto(entity.Id, entity.Name, entity.Description, entity.NeighborId));
  }

  private static async Task<Results<Created<StuffDto>, BadRequest<string>>> CreateAsync(
      CreateStuffRequest request,
      IStuffRepository repo)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
      return TypedResults.BadRequest("Name required.");
    if (request.NeighborId <= 0)
      return TypedResults.BadRequest("NeighborId must be > 0.");

    var entity = new Stuff(
        request.Name.Trim(),
        request.Description?.Trim() ?? string.Empty,
        request.NeighborId
    );

    await repo.AddAsync(entity);
    await repo.SaveChangesAsync();

    var dto = new StuffDto(entity.Id, entity.Name, entity.Description, entity.NeighborId);
    return TypedResults.Created($"/api/stuffs/{entity.Id}", dto);
  }

  private static async Task<Results<Ok<StuffDto>, NotFound, BadRequest<string>>> UpdateAsync(
      long id,
      UpdateStuffRequest request,
      IStuffRepository repo)
  {
    var entity = await repo.GetByIdAsync(id);
    if (entity is null) return TypedResults.NotFound();

    if (string.IsNullOrWhiteSpace(request.Name))
      return TypedResults.BadRequest("Name required.");

    // Comme propriétés privées: recréer un nouvel objet n’est pas souhaitable (perte tracking).
    // On met à jour via réflexion minimale ou expose un setter interne.
    // Ici on utilise un pattern simple: mapper sur les champs via un constructeur + copie.
    // Si besoin, ajuster l'entité pour ajouter une méthode Update.
    entity.GetType().GetProperty(nameof(Stuff.Name))!.SetValue(entity, request.Name.Trim());
    entity.GetType().GetProperty(nameof(Stuff.Description))!.SetValue(entity, request.Description?.Trim() ?? string.Empty);

    repo.Update(entity);
    await repo.SaveChangesAsync();

    var dto = new StuffDto(entity.Id, entity.Name, entity.Description, entity.NeighborId);
    return TypedResults.Ok(dto);
  }

  private static async Task<Results<NoContent, NotFound>> DeleteAsync(long id, IStuffRepository repo)
  {
    var entity = await repo.GetByIdAsync(id);
    if (entity is null) return TypedResults.NotFound();

    repo.Delete(entity);
    await repo.SaveChangesAsync();
    return TypedResults.NoContent();
  }

  // DTOs / Contracts
  public record StuffDto(long Id, string Name, string Description, long NeighborId);
  public record CreateStuffRequest(string Name, string? Description, long NeighborId);
  public record UpdateStuffRequest(string Name, string? Description);
}