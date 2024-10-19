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

        [HttpGet]
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
                var response = APIResponse<IEnumerable<ReviewDTO>>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpGet("{id:int}", Name = "GetReview")]
        public async Task<IActionResult> GetReview(int id)
        {
            if (id <= 0)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { "Id must be greater than 0" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                if (review == null)
                {
                    var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                    { "Review not found" }, HttpStatusCode.NotFound);

                    return NotFound(response);
                }

                var successResponse = APIResponse<ReviewDTO>.CreateSuccessResponse(review);
                return Ok(successResponse);
            }

            catch (Exception ex)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpPost(Name = "CreateReview")]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                        { "Invalid model" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            var movie = await _movieService.GetMovieByIdAsync(reviewDTO.MovieId);
            if (movie == null)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                        { "Movie not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                var response = APIResponse<WatchlistDTO>.CreateErrorResponse(new List<string>
                        { "User not found" }, HttpStatusCode.NotFound);

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
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                     { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }


        [HttpPut("{id:int}", Name = "UpdateReview")]
        [Authorize]
        public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] UpdateReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { "Invalid model" }, HttpStatusCode.BadRequest);

                return BadRequest(response);
            }

            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { "Review not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { "User not found" }, HttpStatusCode.NotFound);

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
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { ex.Message });

                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteReview")]
        [Authorize]
        public async Task<IActionResult> DeleteReview([FromRoute] int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { "Review not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string>
                { "User not found" }, HttpStatusCode.NotFound);

                return NotFound(response);
            }

            try
            {
                await _reviewService.DeleteReviewAsync(id, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<ReviewDTO>.CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.InternalServerError);
                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}