using IMDbClone.Core.Entities;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task UpdateAsync(Rating rating);
    }
}
