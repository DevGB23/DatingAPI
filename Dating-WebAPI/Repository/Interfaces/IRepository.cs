using System.Linq.Expressions;

namespace Dating_WebAPI.Repository.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null, Expression<Func<T, bool>>? filter = null);
    Task<T> GetAsync(string? includeProperties = null, bool tracked = false, Expression<Func<T, bool>>? filter = null);
    Task SaveAsync();
}

