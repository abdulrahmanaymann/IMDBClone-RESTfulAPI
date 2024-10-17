using IMDbClone.Core.Entities;

namespace IMDbClone.DataAccess.Repository.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<bool> IsUniqueUser(string username);
    }
}