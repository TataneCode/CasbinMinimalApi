using CasbinMinimalApi.Domain;
using CasbinMinimanApi.Persistence.Scissors;
using Microsoft.EntityFrameworkCore;

namespace CasbinMinimalApi.Application.Repositories;

public abstract class ScissorsGenericRepository<T>(ScissorsDbContext context) : IScissorsGenericRepository<T> where T : Entity
{

  public async Task<IEnumerable<T>> GetAllAsync()
  {
    return await context.Set<T>().ToListAsync();
  }

  public async Task<T?> GetByIdAsync(long id)
  {
    return await context.Set<T>().FindAsync(id);
  }

  public async Task AddAsync(T entity)
  {
    entity.CreatedOn = DateTime.UtcNow;
    await context.Set<T>().AddAsync(entity);
  }

  public void Update(T entity)
  {
    entity.ModifiedOn = DateTime.UtcNow;
    context.Set<T>().Update(entity);
  }

  public void Delete(T entity)
  {
    context.Set<T>().Remove(entity);
  }

  public async Task SaveChangesAsync()
  {
    await context.SaveChangesAsync();
  }
}