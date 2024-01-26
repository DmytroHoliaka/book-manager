using BookManager.Database;
using BookManager.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Filtering
{
    public class Searcher
    {
        public IEnumerable<Book>? FillteredBook
            => _fillteredBook?.Select(book => new Book(book));

        private IEnumerable<Book>? _fillteredBook;
        private readonly Filter _filter;
        private readonly ApplicationContext _db;

        public Searcher(Filter? filter, ApplicationContext db)
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter), "Filter cannot be null");
            }

            if (db is null)
            {
                throw new ArgumentNullException(nameof(db), "Application context cannot be null");
            }

            _filter = filter;
            _db = db;

            Initialize();
        }

        private void Initialize()
        {
            _fillteredBook = _db.Books
                                    .Include(b => b.Genre)
                                    .Include(b => b.Author)
                                    .Include(b => b.Publisher)
                                    .Where(b => (_filter.Title == string.Empty || b.Title == _filter.Title) &&
                                                (_filter.Genre == string.Empty || b.Genre!.Name == _filter.Genre) &&
                                                (_filter.Author == string.Empty || (b.Author != null && b.Author.Name == _filter.Author)) &&
                                                (_filter.Publisher == string.Empty || (b.Publisher != null && b.Publisher.Name == _filter.Publisher)) &&
                                                (_filter.MoreThanPages == null || b.Pages > _filter.MoreThanPages) &&
                                                (_filter.LessThanPages == null || b.Pages < _filter.LessThanPages) &&
                                                (_filter.PublishedBefore == null || b.ReleaseDate < _filter.PublishedBefore) &&
                                                (_filter.PublishedAfter == null || b.ReleaseDate > _filter.PublishedAfter)
                                           );
        }
    }
}
