using System.Net;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace IMDbClone.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var movies = await _movieService.GetAllMoviesAsync();
                var response = APIResponse<IEnumerable<MovieDTO>>.CreateSuccessResponse(movies);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<MovieDTO>>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpGet("{id:int}", Name = "GetMovie")]
        public async Task<IActionResult> GetMovie(int id)
        {
            if (id <= 0)
            {
                var response = APIResponse<MovieDTO>.CreateErrorResponse(new List<string> { "Id must be greater than 0" }, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var movie = await _movieService.GetMovieByIdAsync(id);
                if (movie == null)
                {
                    var response = APIResponse<MovieDTO>.CreateErrorResponse(new List<string> { "Movie not found" }, HttpStatusCode.NotFound);
                    return NotFound(response);
                }

                var successResponse = APIResponse<MovieDTO>.CreateSuccessResponse(movie);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<MovieDTO>.CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.InternalServerError);
                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpPost(Name = "CreateMovie")]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDTO movieDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var response = APIResponse<CreateMovieDTO>.CreateErrorResponse(errors, HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            try
            {
                var createdMovie = await _movieService.CreateMovieAsync(movieDTO);
                var successResponse = APIResponse<MovieDTO>.CreateSuccessResponse(createdMovie, HttpStatusCode.Created);
                return CreatedAtRoute(nameof(GetMovie), new { id = createdMovie.Id }, successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<CreateMovieDTO>.CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.InternalServerError);
                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpPut("{id:int}", Name = "UpdateMovie")]
        public async Task<IActionResult> UpdateMovie([FromRoute] int id, [FromBody] UpdateMovieDTO movieDTO)
        {
            if (!ModelState.IsValid)
            {
                var response = APIResponse<UpdateMovieDTO>.CreateErrorResponse(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList(), HttpStatusCode.BadRequest);
                return BadRequest(response);
            }

            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                var response = APIResponse<UpdateMovieDTO>.CreateErrorResponse(new List<string> { "Movie not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            try
            {
                await _movieService.UpdateMovieAsync(id, movieDTO);
                var successResponse = APIResponse<UpdateMovieDTO>.CreateSuccessResponse(movieDTO, HttpStatusCode.OK);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = APIResponse<UpdateMovieDTO>.CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.InternalServerError);
                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpDelete("{id:int}", Name = "DeleteMovie")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                var response = APIResponse<MovieDTO>.CreateErrorResponse(new List<string> { "Movie not found" }, HttpStatusCode.NotFound);
                return NotFound(response);
            }

            try
            {
                await _movieService.DeleteMovieAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = APIResponse<MovieDTO>.CreateErrorResponse(new List<string> { ex.Message }, HttpStatusCode.InternalServerError);
                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}