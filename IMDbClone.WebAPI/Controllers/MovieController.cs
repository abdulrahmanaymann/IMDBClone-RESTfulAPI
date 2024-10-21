using System.Linq.Expressions;
using System.Net;
using IMDbClone.Business.Services.IServices;
using IMDbClone.Common;
using IMDbClone.Core.DTOs.MovieDTOs;
using IMDbClone.Core.Entities;
using IMDbClone.Core.Enums;
using IMDbClone.Core.Responses;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? genre,
            [FromQuery] string? director,
            [FromQuery] string? language,
            [FromQuery] DateTime? releaseDate,
            [FromQuery] string? sortBy,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filter = PredicateBuilder.New<Movie>(true);
                bool isValidFilter = true;

                if (!string.IsNullOrEmpty(search))
                {
                    filter = filter.And(m => m.Title.Contains(search) || m.Synopsis.Contains(search));
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    if (Enum.TryParse<Genre>(genre, true, out var genreEnum))
                    {
                        filter = filter.And(m => m.Genre == genreEnum);
                    }
                    else
                    {
                        isValidFilter = false;
                    }
                }

                if (!string.IsNullOrEmpty(director))
                {
                    filter = filter.And(m => m.Director == director);
                }

                if (!string.IsNullOrEmpty(language))
                {
                    filter = filter.And(m => m.Language == language);
                }

                if (releaseDate.HasValue)
                {
                    filter = filter.And(m => m.ReleaseDate.Date == releaseDate.Value.Date);
                }

                Expression<Func<Movie, object>>? orderByExpression = null;
                if (!string.IsNullOrEmpty(sortBy))
                {
                    orderByExpression = sortBy.ToLower() switch
                    {
                        "title" => m => m.Title,
                        "releasedate" => m => m.ReleaseDate,
                        "genre" => m => m.Genre,
                        "director" => m => m.Director,
                        "language" => m => m.Language,
                        _ => null
                    };

                    if (orderByExpression == null)
                    {
                        isValidFilter = false;
                    }
                }

                if (!isValidFilter)
                {
                    return Ok(APIResponse<IEnumerable<MovieDTO>>.CreateSuccessResponse(new List<MovieDTO>(),
                        HttpStatusCode.NoContent));
                }

                var movies = await _movieService.GetAllMoviesAsync(
                    filter: filter,
                    orderByExpression: orderByExpression,
                    isAscending: isAscending,
                    pageNumber: pageNumber,
                    pageSize: pageSize
                );

                return Ok(APIResponse<IEnumerable<MovieDTO>>.CreateSuccessResponse(movies.Items));
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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
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

        [HttpGet("top-rated/{count}", Name = "GetTopRatedMovies")]
        public async Task<IActionResult> GetTopRatedMovies([FromQuery] int count = 10)
        {
            try
            {
                var movies = await _movieService.GetTopRatedMoviesAsync(count);
                return Ok(APIResponse<IEnumerable<MovieDTO>>.CreateSuccessResponse(movies));
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<MovieDTO>>.CreateErrorResponse(new List<string>
                { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        [HttpGet("most-popular/{count}", Name = "GetMostPopularMovies")]
        public async Task<IActionResult> GetMostPopularMovies([FromQuery] int count = 10)
        {
            try
            {
                var movies = await _movieService.GetMostPopularMoviesAsync(count);
                return Ok(APIResponse<IEnumerable<MovieDTO>>.CreateSuccessResponse(movies));
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<MovieDTO>>.CreateErrorResponse(new List<string>
                { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }
    }
}