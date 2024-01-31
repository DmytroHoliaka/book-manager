using BookManager.Database;
using BookManager.Entities;
using BookManager.Filtering;
using Microsoft.Extensions.Configuration;

namespace BookManager.Tests.UnitTesting.xUnit
{
    public class SearcherTests : IDisposable
    {
        private readonly ApplicationContext _db;
        private readonly Searcher _searcher;

        public SearcherTests()
        {
            Dictionary<string, string?>? connection = new()
            {
                {"ConnectionStrings:Default",
                @"Server=(localdb)\mssqllocaldb;Database=DatabaseSearcherTests;Trusted_Connection=True" }
            };

            IConfiguration config = new ConfigurationBuilder()
                                            .AddInMemoryCollection(connection)
                                            .Build();

            _db = new(config);
            _searcher = new(_db);

            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();

            InsertInitialData();
        }

        public void Dispose()
        {
            if (_db.Database.CanConnect() == true)
            {
                _db.Database.EnsureDeleted();
                _db.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        [Fact]
        public async void GetFilteredBooks_InvalidDatabaseContext_ThrowsInvalidOperationException()
        {
            // Arrange
            Filter filter = new();
            _db.Database.EnsureDeleted();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _searcher.GetFilteredBooks(filter));
        }

        [Fact]
        public async void GetFilteredBooks_Null_ThrowsArgumentNullException()
        {
            // Arrange
            Filter? filter = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _searcher.GetFilteredBooks(filter));
        }

        [Theory]
        [MemberData(nameof(TestData.GetFilteredBooks), MemberType = typeof(TestData))]
        public async void GetFilteredBooks_AllColumnsFilter_ReturnsCorrectBooks(List<Guid> expectedIds, Filter filter)
        {
            // Act
            List<Book> filteredBooks = await _searcher.GetFilteredBooks(filter);

            // Assert
            Assert.True(AreBooksEqual(expectedIds, filteredBooks.Select(b => b.Id).ToList()));
        }

        private bool AreBooksEqual(List<Guid> expectedIds, List<Guid> actualIds)
        {
            if (expectedIds.Count != actualIds.Count)
            {
                return false;
            }

            List<Guid> sortedExpectedIds = expectedIds.OrderBy(i => i).ToList();
            List<Guid> sortedactualIds = actualIds.OrderBy(i => i).ToList();

            return sortedExpectedIds.SequenceEqual(sortedactualIds);
        }

        private void InsertInitialData()
        {
            Author alex = new() { Id = Guids.Id1, Name = "Alex" };
            Author john = new() { Id = Guids.Id2, Name = "John" };

            Genre fiction = new() { Id = Guids.Id3, Name = "Fiction" };
            Genre fantasy = new() { Id = Guids.Id4, Name = "Fantacy" };

            Publisher harperCollins = new() { Id = Guids.Id5, Name = "HarperCollins" };
            Publisher scholastic = new() { Id = Guids.Id6, Name = "Scholastic" };

            Book hobit = new()
            {
                Id = Guids.Id7,
                Title = "The Hobbit",
                Pages = 176,
                ReleaseDate = new DateTime(1947, 2, 1),
                Genre = fiction,
                Author = alex,
                Publisher = scholastic,
            };

            Book animalFarm = new()
            {
                Id = Guids.Id8,
                Title = "Animal Farm",
                Pages = 324,
                ReleaseDate = new DateTime(1995, 5, 6),
                Genre = fantasy,
                Author = alex,
                Publisher = harperCollins,
            };

            Book mobyDick = new()
            {
                Id = Guids.Id9,
                Title = "Moby-Dick",
                Pages = 561,
                ReleaseDate = new DateTime(1999, 2, 9),
                Genre = fiction,
                Author = john,
                Publisher = scholastic,
            };

            Book donQuixote = new()
            {
                Id = Guids.Id10,
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

        private class TestData
        {
            public static IEnumerable<object[]> GetFilteredBooks()
            {
                Filter filter1 = new();

                Filter filter2 = new()
                {
                    Title = "The Hobbit",
                };

                Filter filter3 = new()
                {
                    Author = "Alex",
                };

                Filter filter4 = new()
                {
                    Genre = "Fiction",
                };

                Filter filter5 = new()
                {
                    Publisher = "Scholastic",
                };

                Filter filter6 = new()
                {
                    MoreThanPages = 175,
                };

                Filter filter7 = new()
                {
                    LessThanPages = 177,
                };

                Filter filter8 = new()
                {
                    PublishedBefore = new DateTime(1947, 2, 2),
                };

                Filter filter9 = new()
                {
                    PublishedAfter = new DateTime(1947, 1, 31),
                };

                Filter filter10 = new()
                {
                    Title = "The Hobbit",
                    Genre = "Fiction",
                    Author = "Alex",
                    Publisher = "Scholastic",
                    MoreThanPages = 175,
                    LessThanPages = 177,
                    PublishedBefore = new DateTime(1947, 2, 2),
                    PublishedAfter = new DateTime(1947, 1, 31),
                };


                List<Guid> ids1 = [Guids.Id7, Guids.Id8, Guids.Id9, Guids.Id10];
                List<Guid> ids2 = [Guids.Id7];
                List<Guid> ids3 = [Guids.Id7, Guids.Id8];
                List<Guid> ids4 = [Guids.Id7, Guids.Id9];
                List<Guid> ids5 = [Guids.Id7, Guids.Id9];
                List<Guid> ids6 = [Guids.Id7, Guids.Id8, Guids.Id9, Guids.Id10];
                List<Guid> ids7 = [Guids.Id7];
                List<Guid> ids8 = [Guids.Id7, Guids.Id10];
                List<Guid> ids9 = [Guids.Id7, Guids.Id8, Guids.Id9];
                List<Guid> ids10 = [Guids.Id7];

                return new List<object[]>()
                {
                    new object[] { ids1, filter1 },
                    new object[] { ids2, filter2 },
                    new object[] { ids3, filter3 },
                    new object[] { ids4, filter4 },
                    new object[] { ids5, filter5 },
                    new object[] { ids6, filter6 },
                    new object[] { ids7, filter7 },
                    new object[] { ids8, filter8 },
                    new object[] { ids9, filter9 },
                    new object[] { ids10, filter10 },
                };
            }
        }
    }
}
