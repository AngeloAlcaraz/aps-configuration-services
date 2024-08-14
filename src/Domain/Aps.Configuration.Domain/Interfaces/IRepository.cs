namespace Aps.Configuration.Core.Interfaces;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(string id);

    Task<IEnumerable<T>> GetAllAsync(int limit = 1000);

    Task<T> AddAsync(T entity);

    Task<T> UpdateAsync(string id, T entity);

    Task<bool> DeleteAsync(string id);

    Task<(IEnumerable<T> Documents, int TotalCount)> FindAsync(FilterDefinition<T> filter, int pageNumber = 1, int pageSize = 10);
}