namespace BookManager.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public string? Name { get; set; }   // Required

        public ICollection<Book>? Books { get; set; }

        public Publisher()
        {

        }

        public Publisher(Publisher? origin)
        {
            if (origin is null)
            {
                throw new ArgumentNullException(nameof(origin), "Origin instance of publisher cannot be null");
            }

            Id = origin.Id;
            Name = origin.Name;

            Books = origin.Books is null ? null : new List<Book>(origin.Books);
        }
    }
}
