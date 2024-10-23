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

        /// <summary>
        /// Retrieves all ratings from the database.
        /// </summary>
        /// <returns>An API response containing a list of all ratings.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                var response = APIResponse<IEnumerable<RatingDTO>>
                    .CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Retrieves a specific rating by its ID.
        /// </summary>
        /// <param name="id">The ID of the rating to retrieve.</param>
        /// <returns>An API response containing the requested rating.</returns>
        [HttpGet("{id:int}", Name = "GetRating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRating(int id)
        {
            if (id <= 0)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(
                    new List<string> { "Id must be greater than 0" }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var rating = await _ratingService.GetRatingByIdAsync(id);
                if (rating == null)
                {
                    var response = APIResponse<RatingDTO>
                        .CreateErrorResponse(new List<string> { "Rating not found" }, HttpStatusCode.NotFound);
                    return NotFound(response);
                }

                var successResponse = APIResponse<RatingDTO>.CreateSuccessResponse(rating);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Creates a new rating.
        /// </summary>
        /// <param name="ratingDTO">The rating data transfer object containing the rating information.</param>
        /// <returns>An API response indicating the result of the creation.</returns>
        [HttpPost(Name = "CreateRating")]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingDTO ratingDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<CreateRatingDTO>
                    .CreateErrorResponse(new List<string> { "Invalid model object" }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            var movie = await _movieService.GetMovieByIdAsync(ratingDTO.MovieId);
            if (movie == null)
            {
                var response = APIResponse<CreateRatingDTO>
                    .CreateErrorResponse(new List<string> { "Movie not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    var userResponse = APIResponse<CreateRatingDTO>
                        .CreateErrorResponse(new List<string> { "User not found" }, HttpStatusCode.NotFound);
                    return NotFound(userResponse);
                }

                var createdRating = await _ratingService.CreateRatingAsync(ratingDTO, userId);
                var response = APIResponse<CreateRatingDTO>.CreateSuccessResponse(ratingDTO);
                return CreatedAtRoute(nameof(GetRating), new { id = createdRating.Id }, response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<CreateRatingDTO>
                    .CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Updates an existing rating.
        /// </summary>
        /// <param name="id">The ID of the rating to update.</param>
        /// <param name="ratingDTO">The rating data transfer object containing updated rating information.</param>
        /// <returns>An API response indicating the result of the update.</returns>
        [HttpPut("{id:int}", Name = "UpdateRating")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRating([FromRoute] int id, [FromBody] UpdateRatingDTO ratingDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<UpdateRatingDTO>
                    .CreateErrorResponse(new List<string> { "Invalid model object" }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            var rating = await _ratingService.GetRatingByIdAsync(id);
            if (rating == null)
            {
                var response = APIResponse<UpdateRatingDTO>
                    .CreateErrorResponse(new List<string> { "Rating not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<UpdateRatingDTO>
                    .CreateErrorResponse(new List<string> { "User not found" }, HttpStatusCode.NotFound);
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
                var response = APIResponse<UpdateRatingDTO>
                    .CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Deletes a specific rating by its ID.
        /// </summary>
        /// <param name="id">The ID of the rating to delete.</param>
        /// <returns>An API response indicating the result of the deletion.</returns>
        [HttpDelete("{id:int}", Name = "DeleteRating")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _ratingService.GetRatingByIdAsync(id);
            if (rating == null)
            {
                var response = APIResponse<RatingDTO>
                    .CreateErrorResponse(new List<string> { "Rating not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<RatingDTO>
                    .CreateErrorResponse(new List<string> { "User not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            try
            {
                await _ratingService.DeleteRatingAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<RatingDTO>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}