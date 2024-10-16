using System.Linq.Expressions;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task AddAsync(T entity);

        Task RemoveAsync(T entity);

        Task RemoveRangeAsync(IEnumerable<T> entities);

        Task SaveChangesAsync();
    }
}
