using System.Linq.Expressions;
using IMDbClone.Core.Utilities;
using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace IMDbClone.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        internal DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        internal static IQueryable<T> ApplyIncludes(string? includeProperties, IQueryable<T> query)
        {
            if (!string.IsNullOrEmpty(includeProperties))
            {
                var includeList = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var include in includeList)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        public async Task<PaginatedResult<T>> GetAllAsync(
                 Expression<Func<T, bool>>? filter = null,
                 string? includeProperties = null,
                 Expression<Func<T, object>>? orderByExpression = null,
                 bool isAscending = true,
                 int pageNumber = 1,
                 int pageSize = 10,
                 bool trackChanges = true)
        {
            IQueryable<T> query = trackChanges ? _dbSet : _dbSet.AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                query = ApplyIncludes(includeProperties, query);
                query = query.AsSplitQuery();
            }

            query = orderByExpression != null
                ? (isAscending ? query.OrderBy(orderByExpression) : query.OrderByDescending(orderByExpression))
                : query.OrderBy(x => x);

            var totalCount = await query.CountAsync();

            var items = await query
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null, bool trackChanges = true)
        {
            IQueryable<T> query = trackChanges ? _dbSet : _dbSet.AsNoTracking();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = ApplyIncludes(includeProperties, query);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            return await _dbSet.CountAsync(filter);
        }
    }
}