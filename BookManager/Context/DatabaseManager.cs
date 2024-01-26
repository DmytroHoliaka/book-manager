using BookManager.Entities;

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
            Author? duplicate = _db.Authors.FirstOrDefault(a => a.Name == authorName);

            if (duplicate is not null)
            {
                return duplicate.Id;
            }

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
            Genre? duplicate = _db.Genres.FirstOrDefault(d => d.Name == genreName);

            if (duplicate is not null)
            {
                return duplicate.Id;
            }

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
            Publisher? duplicate = _db.Publishers.FirstOrDefault(p => p.Name == publisherName);

            if (duplicate is not null)
            {
                return duplicate.Id;
            }

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
            Book? duplicate = _db.Books.FirstOrDefault(b => b.Title == title &&
                                                           b.Pages == pages &&
                                                           b.ReleaseDate == releaseDate &&
                                                           b.GenreId == genreId &&
                                                           b.AuthorId == authorId &&
                                                           b.PublisherId == publisherId);

            if (duplicate is not null)
            {
                return duplicate.Id;
            }

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
