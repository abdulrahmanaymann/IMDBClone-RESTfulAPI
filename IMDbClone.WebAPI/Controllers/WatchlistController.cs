using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.WatchlistDTOs;
using IMDbClone.Core.Models;
using IMDbClone.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDbClone.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistService _watchlistService;

        public WatchlistController(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        /// <summary>
        /// Retrieves all watchlists for the authenticated user, with optional search, pagination, and error handling.
        /// </summary>
        /// <param name="search">Optional search term to filter watchlists by movie title.</param>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The number of items per page for pagination (default is 10).</param>
        /// <returns>A list of watchlists for the authenticated user.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<WatchlistDTO>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<WatchlistDTO>>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<WatchlistDTO>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    var errorResponse = APIResponse<IEnumerable<WatchlistDTO>>.CreateErrorResponse(
                        new List<string> { "User ID is not found. Please authenticate." },
                        HttpStatusCode.Unauthorized
                    );
                    return StatusCode((int)errorResponse.StatusCode, errorResponse);
                }

                Expression<Func<Watchlist, bool>>? filter = null;
                if (!string.IsNullOrEmpty(search))
                {
                    filter = w => w.UserId == userId && w.Movie.Title.Contains(search);
                }
                else
                {
                    filter = w => w.UserId == userId;
                }

                var watchlists = await _watchlistService.GetAllWatchlistsAsync(userId, filter, pageNumber, pageSize);

                var response = APIResponse<IEnumerable<WatchlistDTO>>.CreateSuccessResponse(watchlists.Items);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<WatchlistDTO>>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Retrieves a specific watchlist by its ID for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the watchlist to retrieve.</param>
        /// <returns>The details of the requested watchlist.</returns>
        [HttpGet("{id:int}", Name = "GetWatchlist")]
        [ProducesResponseType(typeof(APIResponse<WatchlistDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<WatchlistDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<WatchlistDTO>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<WatchlistDTO>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWatchList(int id)
        {
            if (id <= 0)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { "Id must be greater than 0" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { "User not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            try
            {
                var watchlist = await _watchlistService.GetWatchlistByIdAsync(id, userId);
                if (watchlist == null)
                {
                    var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                    { "Watchlist not found" }, HttpStatusCode.NotFound);

                    return NotFound(response);
                }

                var successResponse = APIResponse<WatchlistDTO>.CreateSuccessResponse(watchlist);

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Creates a new watchlist for the authenticated user based on the provided data transfer object.
        /// </summary>
        /// <param name="watchlistDTO">The data transfer object containing watchlist details.</param>
        /// <returns>The created watchlist information, including its ID.</returns>
        [HttpPost(Name = "CreateWatchlist")]
        [ProducesResponseType(typeof(APIResponse<CreateWatchlistDTO>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse<CreateWatchlistDTO>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<CreateWatchlistDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<CreateWatchlistDTO>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateWatchList([FromBody] CreateWatchlistDTO watchlistDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

                var response = APIResponse<CreateWatchlistDTO>.CreateErrorResponse(new List<string>
                { "Invalid model object" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { "User not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            try
            {
                var createdWatchList = await _watchlistService.CreateWatchlistAsync(watchlistDTO, userId);

                var response = APIResponse<CreateWatchlistDTO>.CreateSuccessResponse(watchlistDTO);

                return CreatedAtRoute(nameof(GetWatchList), new { id = createdWatchList.Id }, response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Deletes a specific watchlist by its ID for the authenticated user.
        /// </summary>
        /// <param name="id">The ID of the watchlist to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        [HttpDelete("{id:int}", Name = "DeleteWatchlist")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(APIResponse<WatchlistDTO>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<WatchlistDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<WatchlistDTO>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteWatchlist(int id)
        {
            if (id <= 0)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { "Id must be greater than 0" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { "User not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            try
            {
                await _watchlistService.DeleteWatchlistAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}