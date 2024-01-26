namespace BookManager.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }   // Required        
        public int Pages { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        public int? AuthorId { get; set; }
        public Author? Author { get; set; }

        public int? PublisherId { get; set; }
        public Publisher? Publisher { get; set; }

        public Book()
        {

        }

        public Book(Book? origin)
        {
            if (origin is null)
            {
                throw new ArgumentNullException(nameof(origin), "Origin instance of book cannot be null");
            }

            Id = origin.Id;
            Title = origin.Title;
            Pages = origin.Pages;
            ReleaseDate = origin.ReleaseDate;

            GenreId = origin.GenreId;
            Genre = origin.Genre is null ? null : new Genre(origin.Genre);

            AuthorId = origin.AuthorId;
            Author = origin.Author is null ? null : new Author(origin.Author);

            PublisherId = origin.PublisherId;
            Publisher = origin.Publisher is null ? null : new Publisher(origin.Publisher);
        }
    }
}
