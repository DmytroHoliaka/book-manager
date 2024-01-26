namespace BookManager.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string? Name { get; set; }   // Required   

        public ICollection<Book>? Books { get; set; }

        public Genre()
        {

        }

        public Genre(Genre? origin)
        {
            if (origin is null)
            {
                throw new ArgumentNullException(nameof(origin), "Origin instance of genre cannot be null");
            }

            Id = origin.Id;
            Name = origin.Name;

            Books = origin.Books is null ? null : new List<Book>(origin.Books); 
        }
    }
}
