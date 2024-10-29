using IMDbClone.Core.Models;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task UpdateAsync(Rating rating);
    }
}
