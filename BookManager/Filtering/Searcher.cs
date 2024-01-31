using BookManager.Database;
using BookManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Filtering
{
    public class Searcher
    {
        private readonly ApplicationContext _db;

        public Searcher(ApplicationContext db)
        {
            if (db is null)
            {
                throw new ArgumentNullException(nameof(db), "Application context cannot be null");
            }

            _db = db;
        }

        public Task<List<Book>> GetFilteredBooks(Filter? filter)
        {
            if (_db.Database.CanConnect() == false)
            {
                throw new InvalidOperationException("Cannot get access to database");
            }

            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter), "Filter cannot be null");
            }

            IQueryable<Book> filteredBooks = _db.Books
                                                    .Include(b => b.Genre)
                                                    .Include(b => b.Author)
                                                    .Include(b => b.Publisher);

            if (string.IsNullOrWhiteSpace(filter.Title) == false)
            {
                filteredBooks = filteredBooks.Where(b => b.Title == filter.Title);
            }

            if (string.IsNullOrWhiteSpace(filter.Genre) == false)
            {
                filteredBooks = filteredBooks.Where(b => b.Genre!.Name == filter.Genre);
            }

            if (string.IsNullOrWhiteSpace(filter.Author) == false)
            {
                filteredBooks = filteredBooks.Where(b => b.Author!.Name == filter.Author);
            }

            if (string.IsNullOrWhiteSpace(filter.Publisher) == false)
            {
                filteredBooks = filteredBooks.Where(b => b.Publisher!.Name == filter.Publisher);
            }

            if (filter.MoreThanPages != null)
            {
                filteredBooks = filteredBooks.Where(b => b.Pages > filter.MoreThanPages);
            }

            if (filter.LessThanPages != null)
            {
                filteredBooks = filteredBooks.Where(b => b.Pages < filter.LessThanPages);
            }

            if (filter.PublishedBefore != null)
            {
                filteredBooks = filteredBooks.Where(b => b.ReleaseDate < filter.PublishedBefore);
            }

            if (filter.PublishedAfter != null)
            {
                filteredBooks = filteredBooks.Where(b => b.ReleaseDate > filter.PublishedAfter);
            }

            return filteredBooks.ToListAsync();
        }
    }
}
