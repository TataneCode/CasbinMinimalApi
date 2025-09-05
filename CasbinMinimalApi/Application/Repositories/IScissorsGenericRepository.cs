using CasbinMinimalApi.Domain;

namespace CasbinMinimalApi.Application.Repositories;

public interface IScissorsGenericRepository<T> where T : Entity
{
  Task<IEnumerable<T>> GetAllAsync();
  Task<T?> GetByIdAsync(int id);
  Task AddAsync(T entity);
  void Update(T entity);
  void Delete(T entity);
  Task SaveChangesAsync();
}