using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Models;

namespace MovieAPI.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly AppDbContext _context; // Replace with your actual database context


        public MovieRepository(AppDbContext context)// Replace with your actual database context
        {
            _context = context;
        }

        public async Task<Movie> AddMovieAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie?> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return null;

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetMovieAsync()
        {
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<List<Movie>> GetAllAsync()
        {
            // Use EF Core to asynchronously retrieve all movies from the database
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie> UpdateMovieAsync(Movie movie)
        {
            _context.Entry(movie).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return movie;
        }

        public List<Movie> GetAll()
        {
            return _context.Movies.ToList();
        }
    }
}
