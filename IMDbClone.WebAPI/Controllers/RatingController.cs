using System.Net;
using System.Security.Claims;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.RatingDTOs;
using IMDbClone.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDbClone.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        private readonly IMovieService _movieService;

        public RatingController(IRatingService ratingService, IMovieService movieService)
        {
            _ratingService = ratingService;
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ratings = await _ratingService.GetAllRatingsAsync();
                var response = APIResponse<IEnumerable<RatingDTO>>.CreateSuccessResponse(ratings);
                return Ok(response);
            }

            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<RatingDTO>>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpGet("{id:int}", Name = "GetRating")]
        public async Task<IActionResult> GetRating(int id)
        {
            if (id <= 0)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string>
                { "Id must be greater than 0" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            try
            {
                var rating = await _ratingService.GetRatingByIdAsync(id);
                if (rating == null)
                {
                    var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string>
                    { "Rating not found" }, HttpStatusCode.NotFound);

                    return NotFound(response);
                }

                var successResponse = APIResponse<RatingDTO>.CreateSuccessResponse(rating);
                return Ok(successResponse);
            }

            catch (Exception ex)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpPost(Name = "CreateRating")]
        [Authorize]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingDTO ratingDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<CreateRatingDTO>.CreateErrorResponse(new List<string>
                    { "Invalid model object" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            var movie = await _movieService.GetMovieByIdAsync(ratingDTO.MovieId);
            if (movie == null)
            {
                var response = APIResponse<CreateRatingDTO>.CreateErrorResponse(new List<string>
                            { "Movie not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    var userResponse = APIResponse<CreateRatingDTO>.CreateErrorResponse(new List<string>
                            {"User not found" }, HttpStatusCode.NotFound);

                    return NotFound(userResponse);
                }

                var createdRating = await _ratingService.CreateRatingAsync(ratingDTO, userId);

                var response = APIResponse<CreateRatingDTO>.CreateSuccessResponse(ratingDTO);

                return CreatedAtRoute(nameof(GetRating), new { id = createdRating.Id }, response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<CreateRatingDTO>.CreateErrorResponse(new List<string>
        { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpPut("{id:int}", Name = "UpdateRating")]
        [Authorize]
        public async Task<IActionResult> UpdateRating([FromRoute] int id, [FromBody] UpdateRatingDTO ratingDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<UpdateRatingDTO>.CreateErrorResponse(new List<string>
                { "Invalid model object" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            var rating = await _ratingService.GetRatingByIdAsync(id);
            if (rating == null)
            {
                var response = APIResponse<UpdateRatingDTO>.CreateErrorResponse(new List<string>
                { "Rating not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<UpdateRatingDTO>.CreateErrorResponse(new List<string>
                { "User not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            try
            {
                await _ratingService.UpdateRatingAsync(id, ratingDTO, userId);
                var successResponse = APIResponse<UpdateRatingDTO>.CreateSuccessResponse(ratingDTO);

                return Ok(successResponse);
            }

            catch (Exception ex)
            {
                var response = APIResponse<UpdateRatingDTO>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteRating")]
        [Authorize]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _ratingService.GetRatingByIdAsync(id);
            if (rating == null)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string>
                { "Rating not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string>
                { "User not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            try
            {
                await _ratingService.DeleteRatingAsync(id, userId);
                return NoContent();
            }

            catch (Exception ex)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}
