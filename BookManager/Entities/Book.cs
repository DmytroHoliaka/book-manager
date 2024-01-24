namespace BookManager.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }      // ToDo: IsRequired()
        public int Pages { get; set; }
        public DateTime ReleaseDate { get; set; }

        public int GenreId { get; set; }        
        public Genre? Genre { get; set; }

        public int AuthorId { get; set; }       
        public Author? Author { get; set; }

        public int PublisherId { get; set; }   
        public Publisher? Publisher { get; set; }
    }
}
