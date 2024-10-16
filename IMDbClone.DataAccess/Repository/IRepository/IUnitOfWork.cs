namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IMovieRepository Movie { get; }

        IRatingRepository Rating { get; }

        IReviewRepository Review { get; }

        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}
