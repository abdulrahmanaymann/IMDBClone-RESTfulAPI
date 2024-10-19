using System.Linq.Expressions;
using IMDbClone.Core.DTOs.WatchlistDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Utilities;

namespace IMDbClone.Business.Services.IServices
{
    public interface IWatchlistService
    {
        Task<PaginatedResult<WatchlistDTO>> GetAllWatchlistsAsync(
            string userId,
            Expression<Func<Watchlist, bool>>? filter = null,
            int pageNumber = 1,
            int pageSize = 10
            );

        Task<WatchlistDTO> GetWatchlistByIdAsync(int id, string userId);

        Task<WatchlistDTO> CreateWatchlistAsync(CreateWatchlistDTO watchlistDTO, string userId);

        Task DeleteWatchlistAsync(int id, string userId);
    }
}