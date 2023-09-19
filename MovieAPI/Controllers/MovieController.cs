using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Models;
using MovieAPI.Repositories;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private object updatedMovie;

        public MovieController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesAsync()
        {
            var movies = await _movieRepository.GetMovieAsync();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovieAsync(int id)
        {
            var movie = await _movieRepository.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        [HttpPost]
        public async Task<ActionResult<Movie>> CreateMovieAsync(Movie movie)
        {
            var createdMovie = await _movieRepository.AddMovieAsync(movie);
            return CreatedAtAction(nameof(GetMovieAsync), new { id = createdMovie.Id }, createdMovie);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Movie>> PatchMovieAsync(int id, [FromBody] JsonPatchDocument<Movie> patchDocument)
        {
            // Check if the provided ID matches an existing movie in the database
            var existingMovie = await _movieRepository.GetMovieAsync(id);

            if (existingMovie == null)
            {
                return NotFound(); // Return a 404 Not Found response if the movie doesn't exist
            }

            // Apply the patch document to the existing movie
            patchDocument.ApplyTo(existingMovie);

            if (!TryValidateModel(existingMovie))
            {
                return BadRequest(ModelState); // Return a 400 Bad Request response if the patch is invalid
            }

            // Save the changes to the database
            await _movieRepository.UpdateMovieAsync(existingMovie);

            // Return a 200 OK response with the updated movie
            return Ok(existingMovie);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Movie>> UpdateMovieAsync(int id, Movie updatedMovie)
        {
            var existingMovie = await _movieRepository.GetMovieAsync(id);
            if (existingMovie == null)
            {
                return NotFound(); // Return a 404 Not Found response if the movie doesn't exist
            }
            existingMovie.Title = updatedMovie.Title;
            existingMovie.Description = updatedMovie.Description;
            existingMovie.ReleaseDate = updatedMovie.ReleaseDate;
            await _movieRepository.UpdateMovieAsync(existingMovie);
            return Ok(existingMovie);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovieAsync(int id)
        {
            var movie = await _movieRepository.GetMovieAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var deletedMovie = await _movieRepository.DeleteMovieAsync(id);
            return Ok(deletedMovie);
        }

    }
}
