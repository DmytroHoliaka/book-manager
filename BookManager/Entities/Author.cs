namespace BookManager.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string? Name { get; set; }   // ToDo: IsRequired()

        public ICollection<Book>? Books { get; set; }
    }
}
