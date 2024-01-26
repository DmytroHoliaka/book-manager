namespace BookManager.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string? Name { get; set; }   // Required

        public ICollection<Book>? Books { get; set; }

        public Author()
        {

        }

        public Author(Author? origin)
        {
            if (origin is null)
            {
                throw new ArgumentNullException(nameof(origin), "Origin instance of author cannot be null");
            }

            Id = origin.Id;
            Name = origin.Name;

            Books = origin.Books is null ? null : new List<Book>(origin.Books);
        }
    }
}
