using MovieAPI.Models;

namespace MovieAPI.Repositories
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetMovieAsync();
        Task<Movie> GetMovieAsync(int id);

        Task<Movie> AddMovieAsync(Movie movie);
        Task<Movie> UpdateMovieAsync(Movie movie);
        Task<Movie> DeleteMovieAsync(int id);
        Task<List<Movie>> GetAllAsync();
    }
}
