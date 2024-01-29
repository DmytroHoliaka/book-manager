namespace BookManager.Entities
{
    public class Publisher
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }   // Required

        public ICollection<Book>? Books { get; set; }
    }
}
