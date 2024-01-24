using BookManager.Entities;

namespace BookManager.Context
{
    public interface IDatabaseManager
    {
        int InsertAuthor(string authorName);
        int InsertGenre(string genreName);
        int InsertPublisher(string publisherName);
        int InsertBook(string title, int pages, DateTime releaseDate, int genre, int author, int publisher);
    }
}
