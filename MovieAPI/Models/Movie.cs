namespace MovieAPI.Models
{
    public class Movie
    {
        internal object ReleaseDate;
        internal object Description;

        public int Id { get; set; }
        public string? Title { get; set; }
        public long YearOfRelease { get; set; }
        public string Genre { get; set; }
    }
}
