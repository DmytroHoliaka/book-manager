using BookManager.Entities;
using System;

namespace BookManager.Context
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly ApplicationContext _db;

        public DatabaseManager()
        {
            _db = new();
        }


        public int InsertAuthor(string authorName)
        {
            Author author = new()
            {
                Name = authorName,
            };

            _db.Authors.Add(author);
            _db.SaveChanges();

            return author.Id;
        }

        public int InsertGenre(string genreName)
        {
            Genre genre = new()
            {
                Name = genreName,
            };

            _db.Genres.Add(genre);
            _db.SaveChanges();

            return genre.Id;
        }

        public int InsertPublisher(string publisherName)
        {
            Publisher publisher = new()
            {
                Name = publisherName,
            };

            _db.Publishers.Add(publisher);
            _db.SaveChanges();

            return publisher.Id;
        }

        public int InsertBook(string title, int pages, DateTime releaseDate, int genreId, int authorId, int publisherId)
        {
            Book book = new()
            {
                Title = title,
                Pages = pages,
                ReleaseDate = releaseDate,
                GenreId = genreId,
                AuthorId = authorId,
                PublisherId = publisherId,
            };

            _db.Books.Add(book);
            _db.SaveChanges();

            return book.Id;
        }
    }
}
