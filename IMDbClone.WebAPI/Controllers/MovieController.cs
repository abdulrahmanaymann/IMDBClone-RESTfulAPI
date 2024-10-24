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

        /// <summary>
        /// Retrieves all movies based on search and filter criteria such as genre, director, language, and release date.
        /// </summary>
        /// <param name="search">Search keyword for title or synopsis.</param>
        /// <param name="genre">Genre filter (e.g., Action, Drama).</param>
        /// <param name="director">Filter by director's name.</param>
        /// <param name="language">Filter by language.</param>
        /// <param name="releaseDate">Filter by release date.</param>
        /// <param name="sortBy">Field to sort by (title, releaseDate, etc.).</param>
        /// <param name="isAscending">Whether sorting is ascending or descending.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <returns>List of filtered movies.</returns>
        [ProducesResponseType(typeof(APIResponse<IEnumerable<MovieDTO>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<MovieDTO>>), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<MovieDTO>>), (int)HttpStatusCode.InternalServerError)]
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
                    return Ok(APIResponse<IEnumerable<MovieSummaryDTO>>.CreateSuccessResponse(new List<MovieSummaryDTO>(),
                        HttpStatusCode.NoContent));
                }

                var movies = await _movieService.GetAllMoviesAsync(
                    filter: filter,
                    orderByExpression: orderByExpression,
                    isAscending: isAscending,
                    pageNumber: pageNumber,
                    pageSize: pageSize

                );

                return Ok(APIResponse<IEnumerable<MovieSummaryDTO>>.CreateSuccessResponse(movies.Items));
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<MovieDTO>>.CreateErrorResponse(new List<string> { ex.Message });
                return StatusCode((int)response.StatusCode, response);
            }
        }

        /// <summary>
        /// Retrieves a movie by its ID.
        /// </summary>
        /// <param name="id">The movie's ID.</param>
        /// <returns>Movie details.</returns>
        [ProducesResponseType(typeof(APIResponse<MovieDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<MovieDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<MovieDTO>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<MovieDTO>), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Creates a new movie.
        /// </summary>
        /// <param name="movieDTO">Movie data for creation.</param>
        /// <returns>Details of the created movie.</returns>
        [ProducesResponseType(typeof(APIResponse<MovieDTO>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse<CreateMovieDTO>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<CreateMovieDTO>), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Updates an existing movie.
        /// </summary>
        /// <param name="id">The ID of the movie to update.</param>
        /// <param name="movieDTO">Updated movie data.</param>
        /// <returns>Status of the update.</returns>
        [ProducesResponseType(typeof(APIResponse<UpdateMovieDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<UpdateMovieDTO>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<UpdateMovieDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<UpdateMovieDTO>), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Deletes a movie by its ID.
        /// </summary>
        /// <param name="id">The movie's ID.</param>
        /// <returns>Status of the deletion.</returns>
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(APIResponse<MovieDTO>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<MovieDTO>), (int)HttpStatusCode.InternalServerError)]
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

        /// <summary>
        /// Retrieves the top-rated movies.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination. Default is 1.</param>
        /// <param name="pageSize">The number of top-rated movies to retrieve per page. Default is 10.</param>
        /// <returns>List of top-rated movies.</returns>
        [ProducesResponseType(typeof(APIResponse<IEnumerable<MovieSummaryDTO>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<MovieSummaryDTO>>), (int)HttpStatusCode.InternalServerError)]
        [HttpGet("top-rated/{pageNumber:int=1}/{pageSize:int=10}", Name = "GetTopRatedMovies")]
        public async Task<IActionResult> GetTopRatedMovies([FromRoute] int pageNumber = 1, [FromRoute] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var errorResponse = APIResponse<IEnumerable<MovieSummaryDTO>>.CreateErrorResponse(
                    new List<string> { "Page number and page size must be greater than zero." });
                return BadRequest(errorResponse);
            }

            try
            {
                var paginatedResult = await _movieService.GetTopRatedMoviesAsync(pageNumber, pageSize);
                return Ok(APIResponse<IEnumerable<MovieSummaryDTO>>.CreateSuccessResponse(paginatedResult.Items));
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<MovieSummaryDTO>>.CreateErrorResponse(
                    new List<string> { "An error occurred while retrieving top-rated movies.", ex.Message });
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }

        /// <summary>
        /// Retrieves the most popular movies.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination. Default is 1.</param>
        /// <param name="pageSize">The number of most popular movies to retrieve per page. Default is 10.</param>
        /// <returns>List of most popular movies.</returns>
        [ProducesResponseType(typeof(APIResponse<IEnumerable<MovieSummaryDTO>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<MovieSummaryDTO>>), (int)HttpStatusCode.InternalServerError)]
        [HttpGet("most-popular/{pageNumber:int=1}/{pageSize:int=10}", Name = "GetMostPopularMovies")]
        public async Task<IActionResult> GetMostPopularMovies([FromRoute] int pageNumber = 1, [FromRoute] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                var errorResponse = APIResponse<IEnumerable<MovieSummaryDTO>>.CreateErrorResponse(
                    new List<string> { "Page number and page size must be greater than zero." });
                return BadRequest(errorResponse);
            }

            try
            {
                var paginatedResult = await _movieService.GetMostPopularMoviesAsync(pageNumber, pageSize);
                return Ok(APIResponse<IEnumerable<MovieSummaryDTO>>.CreateSuccessResponse(paginatedResult.Items));
            }
            catch (Exception ex)
            {
                var response = APIResponse<IEnumerable<MovieSummaryDTO>>.CreateErrorResponse(new List<string>
                { "An error occurred while retrieving most popular movies.", ex.Message });
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }
        }
    }
}