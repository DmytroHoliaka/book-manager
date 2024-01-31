namespace BookManager.Database
{
    public interface IDatabaseManager
    {
        Task<Guid> InsertAuthorAsync(string authorName);
        Task<Guid> InsertGenreAsync(string genreName);
        Task<Guid> InsertPublisherAsync(string publisherName);
        Task<Guid> InsertBookAsync(string title, int pages, DateTime? releaseDate, Guid genre, Guid? author, Guid? publisher);
    }
}
