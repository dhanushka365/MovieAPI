using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Controllers;
using MovieAPI.Data;
using MovieAPI.Models;
using MovieAPI.Repositories;

namespace APITests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetAllTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "MovieListDatabase")
            .Options;

            // Insert seed data into the database using one instance of the context
            using (var context = new AppDbContext(options))
            {
                context.Movies.Add(new Movie { Id = 1, Title = "Movie 1", YearOfRelease = 2018, Genre = "Action" });
                context.Movies.Add(new Movie { Id = 2, Title = "Movie 2", YearOfRelease = 2018, Genre = "Action" });
                context.Movies.Add(new Movie { Id = 3, Title = "Movie 3", YearOfRelease = 2019, Genre = "Action" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new AppDbContext(options))
            {
                MovieRepository movieRepository = new MovieRepository(context);
                List<Movie> movies = movieRepository.GetAll();
                Assert.AreEqual(3, movies.Count);
            }
        }


        [TestMethod]
        public void PostMovieTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MovieListDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                var movieRepository = new MovieRepository(context);
                var movieController = new MovieController(movieRepository);

                // Act
                var newMovie = new Movie { Title = "New Movie", YearOfRelease = 2022, Genre = "Drama" };
                var result = movieController.CreateMovieAsync(newMovie);
                Console.WriteLine(result);
                // Assert
                Assert.IsNotNull(result);
            
                // You can also retrieve the created movie and check its properties
                var createdMovie = context.Movies.FirstOrDefault(m => m.Title == "New Movie");
                Assert.IsNotNull(createdMovie);
                Assert.AreEqual(newMovie.Title, createdMovie.Title);

                // check new instance is added to database
                List<Movie> movies = movieRepository.GetAll();
                Assert.AreEqual(5, movies.Count);
                
            }
        }


        [TestMethod]
        public void DeleteMovieTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MovieListDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                // Create and add a movie to the database
                var movie = new Movie { Id = 1, Title = "Movie to Delete", YearOfRelease = 2015, Genre = "Action" };
                context.Movies.Add(movie);
                context.SaveChanges();

                var movieRepository = new MovieRepository(context);
                var movieController = new MovieController(movieRepository);

                // Act
                var result = movieController.DeleteMovieAsync(1);

                // Assert
                Assert.IsNotNull(result);
                //Assert.AreEqual(StatusCodes.Status204NoContent, result);

                // Verify that the movie has been deleted from the database
                var deletedMovie = context.Movies.Find(1);
                Assert.IsNull(deletedMovie);
            }
        }


        [TestMethod]
        public void PatchMovieTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MovieListDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                
                var movie = new Movie { Id = 9, Title = "Existing Movie", YearOfRelease = 2010, Genre = "Comedy" };
                context.Movies.Add(movie);
                context.SaveChanges();
                var movieRepository = new MovieRepository(context);
                var movieController = new MovieController(movieRepository);
                // Act
                var patchDoc = new JsonPatchDocument<Movie>();
                patchDoc.Replace(m => m.Title, "Updated Movie");
                var result = movieController.PatchMovieAsync(9, patchDoc);
                // Assert
                Assert.IsNotNull(result);
                //Assert.AreEqual(StatusCodes.Status204NoContent, ((NoContentResult)result).StatusCode);   
                var updatedMovieInDb = context.Movies.Find(9);
                Assert.IsNotNull(updatedMovieInDb);
                Assert.AreEqual("Updated Movie", updatedMovieInDb.Title);
            }
        }


        [TestMethod]
        public void PutMovieTest()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "MovieListDatabase")
                .Options;

            using (var context = new AppDbContext(options))
            {
                // Create and add a movie to the database
                var movie = new Movie { Id = 5, Title = "Existing Movie", YearOfRelease = 2010, Genre = "Comedy" };
                context.Movies.Add(movie);
                context.SaveChanges();

                var movieRepository = new MovieRepository(context);
                var movieController = new MovieController(movieRepository);

                // Act
                var updatedMovie = new Movie { Id = 5, Title = "Updated Movie", YearOfRelease = 2010, Genre = "Comedy" };
                var result = movieController.UpdateMovieAsync(5, updatedMovie);

                // Assert
                Assert.IsNotNull(result);
                //Assert.AreEqual(StatusCodes.Status200OK, ((OkObjectResult)result).StatusCode);

                // Verify that the movie has been updated in the database
                var updatedMovieInDb = context.Movies.Find(5);
                Assert.IsNotNull(updatedMovieInDb);
                Assert.AreEqual("Updated Movie", updatedMovieInDb.Title);
            }
        }




    }
}