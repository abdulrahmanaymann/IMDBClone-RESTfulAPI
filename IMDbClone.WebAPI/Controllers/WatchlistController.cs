using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.DTOs.WatchlistDTOs;
using IMDbClone.Core.Entities;
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
        public readonly IWatchlistService _watchlistService;

        public WatchlistController(IWatchlistService watchlistService)
        {
            _watchlistService = watchlistService;
        }

        [HttpGet]
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


        [HttpGet("{id:int}", Name = "GetWatchlist")]
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

        [HttpPost(Name = "CreateWatchlist")]
        public async Task<IActionResult> CreateWatchList([FromBody] CreateWatchlistDTO watchlistDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

                var response = APIResponse<CreateRatingDTO>.CreateErrorResponse(new List<string>
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

        [HttpDelete("{id:int}", Name = "DeleteWatchlist")]
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
