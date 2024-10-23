using System.Linq.Expressions;
using IMDbClone.Core.Utilities;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<PaginatedResult<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            Expression<Func<T, object>>? orderByExpression = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10,
            bool trackChanges = true
            );

        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool trackChanges = true);

        Task AddAsync(T entity);

        Task RemoveAsync(T entity);

        Task RemoveRangeAsync(IEnumerable<T> entities);

        Task SaveChangesAsync();
    }
}
