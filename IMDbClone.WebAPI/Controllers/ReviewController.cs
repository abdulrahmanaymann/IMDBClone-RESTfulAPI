using System.Net;
using System.Security.Claims;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.ReviewDTOs;
using IMDbClone.Core.DTOs.WatchlistDTOs;
using IMDbClone.Core.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDbClone.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IMovieService _movieService;

        public ReviewController(IReviewService reviewService, IMovieService movieService)
        {
            _reviewService = reviewService;
            _movieService = movieService;
        }

        /// <summary>
        /// Retrieves all reviews from the database.
        /// </summary>
        /// <returns>A list of reviews with a success response or an error response in case of failure.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ReviewDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                var response = APIResponse<IEnumerable<ReviewDTO>>.CreateSuccessResponse(reviews);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<ReviewDTO>>
                    .CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Retrieves a single review by its ID.
        /// </summary>
        /// <param name="id">The ID of the review to retrieve.</param>
        /// <returns>A specific review with a success response or a not found response if the review doesn't exist.</returns>
        [HttpGet("{id:int}", Name = "GetReview")]
        [ProducesResponseType(typeof(APIResponse<ReviewDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<ReviewDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReview(int id)
        {
            if (id <= 0)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(
                    new List<string> { "Id must be greater than 0" }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                if (review == null)
                {
                    var response = APIResponse<ReviewDTO>
                        .CreateErrorResponse(new List<string> { "Review not found" }, HttpStatusCode.NotFound);
                    return NotFound(response);
                }

                var successResponse = APIResponse<ReviewDTO>.CreateSuccessResponse(review);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Creates a new review for a specified movie.
        /// </summary>
        /// <param name="reviewDTO">The review data transfer object containing the review details.</param>
        /// <returns>A created review with a success response or an error response if the movie is not found or other errors occur.</returns>
        [HttpPost(Name = "CreateReview")]
        [Authorize]
        [ProducesResponseType(typeof(APIResponse<ReviewDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { "Invalid model" }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            var movie = await _movieService.GetMovieByIdAsync(reviewDTO.MovieId);
            if (movie == null)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { "Movie not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                var response = APIResponse<WatchlistDTO>
                    .CreateErrorResponse(new List<string> { "User not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            try
            {
                var createdReview = await _reviewService.CreateReviewAsync(reviewDTO, userId);
                var successResponse = APIResponse<ReviewDTO>.CreateSuccessResponse(createdReview);
                return CreatedAtRoute("GetReview", new { id = createdReview.Id }, successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Updates an existing review by its ID.
        /// </summary>
        /// <param name="id">The ID of the review to update.</param>
        /// <param name="reviewDTO">The data transfer object containing the updated review details.</param>
        /// <returns>An updated review with a success response or an error response if the review is not found or validation fails.</returns>
        [HttpPut("{id:int}", Name = "UpdateReview")]
        [Authorize]
        [ProducesResponseType(typeof(APIResponse<UpdateReviewDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<ReviewDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] UpdateReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { "Invalid model" }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { "Review not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { "User not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            try
            {
                await _reviewService.UpdateReviewAsync(id, reviewDTO, userId);
                var response = APIResponse<UpdateReviewDTO>.CreateSuccessResponse(reviewDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Deletes a review by its ID.
        /// </summary>
        /// <param name="id">The ID of the review to delete.</param>
        /// <returns>No content if the deletion is successful or an error response if the review is not found or deletion fails.</returns>
        [HttpDelete("{id:int}", Name = "DeleteReview")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(APIResponse<ReviewDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReview([FromRoute] int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { "Review not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { "User not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            try
            {
                await _reviewService.DeleteReviewAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<ReviewDTO>
                    .CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.InternalServerError);
                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}