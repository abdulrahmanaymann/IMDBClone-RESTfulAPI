using IMDbClone.DataAccess.Data;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace IMDbClone.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDbContextTransaction? _transaction; // Made nullable

        public IMovieRepository Movie { get; private set; }
        public IRatingRepository Rating { get; private set; }
        public IReviewRepository Review { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Movie = new MovieRepository(_context);
            Rating = new RatingRepository(_context);
            Review = new ReviewRepository(_context);
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction!.CommitAsync();
            }
            catch (Exception ex)
            {
                await _transaction!.RollbackAsync();
                throw new Exception("Error occurred while saving changes to the database.", ex);
            }
            finally
            {
                await _transaction!.DisposeAsync();
                _transaction = null; // To avoid reuse
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null; // To avoid reuse
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null; // To avoid reuse
            _context.Dispose();
            GC.SuppressFinalize(this); // To prevent finalizer from running
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null; // To avoid reuse
            }

            await _context.DisposeAsync();
            GC.SuppressFinalize(this); // To prevent finalizer from running
        }
    }
}