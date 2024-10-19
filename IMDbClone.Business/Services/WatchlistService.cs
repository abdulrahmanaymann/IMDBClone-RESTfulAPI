using System.Linq.Expressions;
using AutoMapper;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.WatchlistDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Exceptions;
using IMDbClone.Core.Utilities;
using IMDbClone.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;

namespace IMDbClone.Business.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;

        private readonly ICacheService _cacheService;

        public WatchlistService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<PaginatedResult<WatchlistDTO>> GetAllWatchlistsAsync(
            string userId,
            Expression<Func<Watchlist, bool>>? filter = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var cacheKey = $"AllWatchlists_{userId}";

            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var watchlists = await _unitOfWork.Watchlist.GetAllAsync(
                    filter: filter,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    includeProperties: "Movie"
                );

                return new PaginatedResult<WatchlistDTO>
                {
                    Items = _mapper.Map<IEnumerable<WatchlistDTO>>(watchlists.Items),
                    TotalCount = watchlists.TotalCount,
                    PageNumber = watchlists.PageNumber,
                    PageSize = watchlists.PageSize
                };
            });
        }

        public async Task<WatchlistDTO> GetWatchlistByIdAsync(int id, string userId)
        {
            var cacheKey = $"Watchlist{id}_{userId}";

            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var watchlist = await _unitOfWork.Watchlist.GetAsync(w => w.Id == id && w.UserId == userId,
                    includeProperties: "Movie");

                if (watchlist == null)
                {
                    throw new ApiException($"Watchlist item with ID {id} not found for the user.",
                                        StatusCodes.Status404NotFound);
                }

                return _mapper.Map<WatchlistDTO>(watchlist);
            });
        }

        public async Task<WatchlistDTO> CreateWatchlistAsync(CreateWatchlistDTO watchlistDTO, string userId)
        {
            if (watchlistDTO == null)
            {
                throw new ApiException("Watchlist data cannot be null.", StatusCodes.Status400BadRequest);
            }

            var existingWatchlist = await _unitOfWork.Watchlist.GetAsync(w => w.MovieId == watchlistDTO.MovieId);
            if (existingWatchlist != null)
            {
                throw new ApiException("Movie already exists in the watchlist.", StatusCodes.Status409Conflict);
            }

            var watchlist = _mapper.Map<Watchlist>(watchlistDTO);
            watchlist.UserId = userId;

            await _unitOfWork.Watchlist.AddAsync(watchlist);
            _cacheService.Remove($"AllWatchlists_{userId}");

            return _mapper.Map<WatchlistDTO>(watchlist);
        }

        public async Task DeleteWatchlistAsync(int id, string userId)
        {
            var watchlist = await _unitOfWork.Watchlist.GetAsync(w => w.Id == id && w.UserId == userId);
            if (watchlist == null)
            {
                throw new ApiException($"Watchlist item with ID {id} not found for the user.",
                                StatusCodes.Status404NotFound);
            }

            await _unitOfWork.Watchlist.RemoveAsync(watchlist);
            _cacheService.Remove($"Watchlist{id}_{userId}");
            _cacheService.Remove($"AllWatchlists_{userId}");
        }
    }
}