using BookManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly ApplicationContext _db;

        public DatabaseManager(ApplicationContext? db)
        {
            if (db is null)
            {
                throw new ArgumentNullException(nameof(db), "Application context cannot be null");
            }

            _db = db;
        }

        public async Task<Guid> InsertAuthorAsync(string authorName)
        {
            if (await _db.Database.CanConnectAsync() == false)
            {
                throw new InvalidOperationException("Cannot get access to database");
            }

            Author? duplicate = await _db.Authors.FirstOrDefaultAsync(a => a.Name == authorName);

            if (duplicate is not null)
            {
                return duplicate.Id;
            }

            Author author = new()
            {
                Name = authorName,
            };

            await _db.Authors.AddAsync(author);
            await _db.SaveChangesAsync();

            return author.Id;
        }

        public async Task<Guid> InsertGenreAsync(string genreName)
        {
            if (await _db.Database.CanConnectAsync() == false)
            {
                throw new InvalidOperationException("Cannot get access to database");
            }

            Genre? duplicate = await _db.Genres.FirstOrDefaultAsync(d => d.Name == genreName);

            if (duplicate is not null)
            {
                return duplicate.Id;
            }

            Genre genre = new()
            {
                Name = genreName,
            };

            await _db.Genres.AddAsync(genre);
            await _db.SaveChangesAsync();

            return genre.Id;
        }

        public async Task<Guid> InsertPublisherAsync(string publisherName)
        {
            if (await _db.Database.CanConnectAsync() == false)
            {
                throw new InvalidOperationException("Cannot get access to database");
            }

            Publisher? duplicate = await _db.Publishers.FirstOrDefaultAsync(p => p.Name == publisherName);

            if (duplicate is not null)
            {
                return duplicate.Id;
            }

            Publisher publisher = new()
            {
                Name = publisherName,
            };

            await _db.Publishers.AddAsync(publisher);
            await _db.SaveChangesAsync();

            return publisher.Id;
        }

        public async Task<Guid> InsertBookAsync(string title, int pages, DateTime? releaseDate, Guid genreId, Guid? authorId, Guid? publisherId)
        {
            if (await _db.Database.CanConnectAsync() == false)
            {
                throw new InvalidOperationException("Cannot get access to database");
            }

            Book? duplicate = await _db.Books.FirstOrDefaultAsync(b => b.Title == title &&
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

            await _db.Books.AddAsync(book);
            await _db.SaveChangesAsync();

            return book.Id;
        }
    }
}