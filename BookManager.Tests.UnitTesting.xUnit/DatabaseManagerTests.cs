using BookManager.Database;
using BookManager.Entities;
using Microsoft.Extensions.Configuration;

namespace BookManager.Tests.UnitTesting.xUnit
{
    public class DatabaseManagerTests : IDisposable
    {
        private readonly DatabaseManager _dm;
        private readonly ApplicationContext _db;

        public DatabaseManagerTests()
        {
            Dictionary<string, string?>? connection = new()
            {
                { "ConnectionStrings:Default",
                 @"Server=(localdb)\mssqllocaldb;Database=DatabaseManagerTests;Trusted_Connection=True;" }
            };

            IConfiguration config = new ConfigurationBuilder()
                                        .AddInMemoryCollection(connection)
                                        .Build();
            _db = new(config);
            _dm = new(_db);

            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();

            InsertInitialData();
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Dispose();

            GC.SuppressFinalize(this);
        }

        [Fact]
        public async void InsertAuthorAsync_AuthorName_ReturnsNewAuthor()
        {
            // Arrange
            string authorName = "Dmytro";

            // Act
            await _dm.InsertAuthorAsync(authorName);

            // Assert
            Assert.True(IsAuthorInserted(authorName));
        }

        [Fact]
        public async void InsertAuthorAsync_DuplicateAuthorName_ReturnsExistingEntry()
        {
            // Arrange
            string duplicateAuthorName = "Alex";
            Author author = _db.Authors.First(a => a.Name == duplicateAuthorName);
            int initialCount = 2;

            // Act
            Guid authorId = await _dm.InsertAuthorAsync(duplicateAuthorName);

            // Assert
            Assert.Equal(initialCount, _db.Authors.ToList().Count);
            Assert.Equal(author.Id, authorId);
        }

        [Fact]
        public async void InsertAuthorAsync_InvalidDatabaseContext_ThrowsInvalidOperationException()
        {
            // Arrange
            string authorName = "Dmytro";
            _db.Database.EnsureDeleted();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _dm.InsertAuthorAsync(authorName));
        }

        [Fact]
        public async void InsertGenreAsync_GenreName_ReturnsNewGenre()
        {
            // Arrange
            string genreName = "Thriller";

            // Act
            await _dm.InsertGenreAsync(genreName);

            // Assert
            Assert.True(IsGenreInserted(genreName));
        }

        [Fact]
        public async void InsertGenreAsync_DuplicateGenreName_ReturnsExistingEntry()
        {
            // Arrange
            string duplicateGenreName = "Fiction";
            Genre genre = _db.Genres.First(g => g.Name == duplicateGenreName);
            int initialCount = 2;

            // Act
            Guid genreId = await _dm.InsertGenreAsync(duplicateGenreName);

            // Assert
            Assert.Equal(initialCount, _db.Genres.ToList().Count);
            Assert.Equal(genre.Id, genreId);
        }

        [Fact]
        public async void InsertGenreAsync_InvalidDatabaseContext_ThrowsInvalidOperationException()
        {
            // Arrange
            string genreName = "Thriller";
            _db.Database.EnsureDeleted();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _dm.InsertGenreAsync(genreName));
        }

        [Fact]
        public async void InsertPublisherAsync_PublisherName_ReturnsNewPublisher()
        {
            // Arrange
            string publisherName = "The Viking Press";

            // Act
            await _dm.InsertPublisherAsync(publisherName);

            // Assert
            Assert.True(IsPublisherInserted(publisherName));
        }

        [Fact]
        public async void InsertPublisherAsync_DuplicatePublisherName_ReturnsExistingEntry()
        {
            // Arrange
            string duplicatePublisherName = "HarperCollins";
            Publisher publisher = _db.Publishers.First(p => p.Name == duplicatePublisherName);
            int initialCount = 2;

            // Act
            Guid publisherId = await _dm.InsertPublisherAsync(duplicatePublisherName);

            // Assert
            Assert.Equal(initialCount, _db.Publishers.ToList().Count);
            Assert.Equal(publisher.Id, publisherId);
        }

        [Fact]
        public async void InsertPublisherAsync_InvalidDatabaseContext_ThrowsInvalidOperationException()
        {
            // Arrange
            string publisherName = "HarperCollins";
            _db.Database.EnsureDeleted();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _dm.InsertPublisherAsync(publisherName));
        }


        [Fact]
        public async void InsertBookAsync_BookInformation_ReturnsNewBook()
        {
            // Arrange
            Genre genre = _db.Genres.First(g => g.Name == "Fiction");

            string title = "The Lord of the Rings";
            int pages = 458;
            DateTime? releaseDate = null;
            Guid genreId =  genre.Id;
            Guid? authorId = null;
            Guid? publisherId = null;

            // Act
            await _dm.InsertBookAsync(title, pages, releaseDate, genreId, authorId, publisherId);

            // Assert
            Assert.True(IsBookInserted(title, pages, releaseDate, genreId, authorId, publisherId));
        }

        [Fact]
        public async void InsertBookAsync_DuplicatePublisherName_ReturnsExistingEntry()
        {
            // Arrange
            Genre genre = _db.Genres.First(g => g.Name == "Fiction");
            Author author = _db.Authors.First(a => a.Name == "Alex");
            Publisher publisher = _db.Publishers.First(p => p.Name == "Scholastic");

            string titleDuplicate = "The Hobbit";
            int pagesDuplicate = 176;
            DateTime? releaseDateDuplicate = new DateTime(1947, 2, 1);
            Guid genreIdDuplicate = genre.Id;
            Guid? authorIdDuplicate = author.Id;
            Guid? publisherIdDuplicate = publisher.Id;

            Book originalBook = _db.Books.First(b => b.Title == titleDuplicate &&
                                                      b.Pages == pagesDuplicate &&
                                                      b.ReleaseDate == releaseDateDuplicate &&
                                                      b.GenreId == genreIdDuplicate &&
                                                      b.AuthorId == authorIdDuplicate &&
                                                      b.PublisherId == publisherIdDuplicate);

            int initialCount = 4;

            // Act
            Guid duplacateBookId = await _dm.InsertBookAsync(titleDuplicate,
                                                             pagesDuplicate,
                                                             releaseDateDuplicate,
                                                             genreIdDuplicate,
                                                             authorIdDuplicate,
                                                             publisherIdDuplicate);

            // Assert
            Assert.Equal(initialCount, _db.Books.ToList().Count);
            Assert.Equal(originalBook.Id, duplacateBookId);
        }

        [Fact]
        public async void InsertBookAsync_InvalidDatabaseContext_ThrowsInvalidOperationException()
        {
            // Arrange
            Genre genre = _db.Genres.First(g => g.Name == "Fiction");

            string titleDuplicate = "The Witcher";
            int pagesDuplicate = 231;
            DateTime? releaseDateDuplicate = new DateTime(1946, 3, 8);
            Guid genreIdDuplicate = genre.Id;
            Guid? authorIdDuplicate = null;
            Guid? publisherIdDuplicate = null;

            _db.Database.EnsureDeleted();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _dm.InsertBookAsync(titleDuplicate,
                                          pagesDuplicate,
                                          releaseDateDuplicate,
                                          genreIdDuplicate,
                                          authorIdDuplicate,
                                          publisherIdDuplicate);
            });
        }



        private bool IsAuthorInserted(string authorName)
        {
            Author? insertedAuthor = _db.Authors.FirstOrDefault(a => a.Name == authorName);
            return insertedAuthor is not null;
        }

        private bool IsGenreInserted(string genreName)
        {
            Genre? insertedGenre = _db.Genres.FirstOrDefault(g => g.Name == genreName);
            return insertedGenre is not null;
        }

        private bool IsPublisherInserted(string publisherName)
        {
            Publisher? publisher = _db.Publishers.FirstOrDefault(p => p.Name == publisherName);
            return publisher is not null;
        }

        private bool IsBookInserted(string title,
                                    int pages,
                                    DateTime? releaseDate,
                                    Guid genreId,
                                    Guid? authorId,
                                    Guid? publisherId)
        {
            Book? book = _db.Books.FirstOrDefault(b => b.Title == title &&
                                                      b.Pages == pages &&
                                                      b.ReleaseDate == releaseDate &&
                                                      b.GenreId == genreId &&
                                                      b.AuthorId == authorId &&
                                                      b.PublisherId == publisherId);
            return book is not null;
        }


        private void InsertInitialData()
        {
            Author alex = new() { Name = "Alex" };
            Author john = new() { Name = "John" };

            Genre fiction = new() { Name = "Fiction" };
            Genre fantasy = new() { Name = "Fantacy" };

            Publisher harperCollins = new() { Name = "HarperCollins" };
            Publisher scholastic = new() { Name = "Scholastic" };

            Book hobit = new()
            {
                Title = "The Hobbit",
                Pages = 176,
                ReleaseDate = new DateTime(1947, 2, 1),
                Genre = fiction,
                Author = alex,
                Publisher = scholastic,
            };

            Book animalFarm = new()
            {
                Title = "Animal Farm",
                Pages = 324,
                ReleaseDate = new DateTime(1995, 5, 6),
                Genre = fantasy,
                Author = alex,
                Publisher = harperCollins,
            };

            Book mobyDick = new()
            {
                Title = "Moby-Dick",
                Pages = 561,
                ReleaseDate = new DateTime(1999, 2, 9),
                Genre = fiction,
                Author = john,
                Publisher = scholastic,
            };

            Book donQuixote = new()
            {
                Title = "Don Quixote",
                Pages = 273,
                ReleaseDate = new DateTime(1867, 1, 29),
                Genre = fantasy,
                Author = john,
                Publisher = harperCollins,
            };

            _db.Authors.AddRange(alex, john);
            _db.Genres.AddRange(fiction, fantasy);
            _db.Publishers.AddRange(harperCollins, scholastic);
            _db.Books.AddRange(hobit, animalFarm, mobyDick, donQuixote);

            _db.SaveChanges();
        }
    }
}
