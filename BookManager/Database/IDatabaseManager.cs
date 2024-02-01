namespace BookManager.Database
{
    public interface IDatabaseManager
    {
        Task<Guid> EnsureInsertedAuthorAsync(string authorName);
        Task<Guid> EnsureInsertedGenreAsync(string genreName);
        Task<Guid> EnsureInsertedPublisherAsync(string publisherName);
        Task<Guid> EnsureInsertedBookAsync(string title, int pages, DateTime? releaseDate, Guid genre, Guid? author, Guid? publisher);
    }
}
