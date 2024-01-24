namespace BookManager.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string? Name { get; set; }   // ToDo: IsRequired()

        public ICollection<Book>? Books { get; set; }
    }
}
