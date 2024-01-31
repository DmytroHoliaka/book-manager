using BookManager.Database;
using BookManager.Entities;
using BookManager.FileProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BookManager.Tests.UnitTesting.xUnit
{
    public class FileParserCSVTests : IDisposable
    {
        private readonly ApplicationContext _db;
        private readonly FileParserCSV _parser;

        private readonly string _dirName;
        private readonly string _fileName;
        private readonly string _path;

        public FileParserCSVTests()
        {
            Dictionary<string, string> connection = new()
            {
                { "ConnectionStrings:Default",
                 @"Server=(localdb)\mssqllocaldb;Database=FileParserCSVTests;Trusted_Connection=True;" }
            };

            IConfiguration config = new ConfigurationBuilder()
                                        .AddInMemoryCollection(connection!)
                                        .Build();
            _db = new(config);
            _parser = new(new DatabaseManager(_db));

            _dirName = "FileParserCSVTests";
            _fileName = "Temp.csv";
            _path = @$".\{_dirName}\{_fileName}";

            Directory.CreateDirectory(_dirName);
            _db.Database.EnsureDeleted();
            _db.Database.EnsureCreated();
        }

        public void Dispose()
        {
            if (_db.Database.CanConnect() == true)
            {
                _db.Database.EnsureDeleted();
                _db.Dispose();
            }

            if (Directory.Exists(_dirName))
            {
                Directory.Delete(_dirName, true);
            }

            GC.SuppressFinalize(this);
        }

        [Theory]
        [MemberData(nameof(TestData.GetJsonFileWithCorrectDate), MemberType = typeof(TestData))]
        public async void ParseAsync_JsonFileWithCorrectDate_ReturnsDateFromDatabase(string content, DbTemplate template)
        {
            // Arrange
            File.WriteAllText(_path, content);

            // Act
            await _parser.ParseAsync(_path);

            // Assert
            Assert.True(IsCorrectFilled(template));
        }

        public record DbTemplate (List<string> Titles,
                                  List<int> Pages,
                                  List<DateTime?> ReleaseDates,
                                  List<string> GenreNames,
                                  List<string> AuthorNames,
                                  List<string> PublisherNames);

        private bool IsCorrectFilled(DbTemplate template)
        {
            List<Book> books = _db.Books
                                    .Include(b => b.Author)
                                    .Include(b => b.Genre)
                                    .Include(b => b.Publisher)
                                    .ToList();

            if (AreTheSame(books.Select(b => b.Title).ToList()!, template.Titles) &&
                AreTheSame(books.Select(b => b.Pages).ToList(), template.Pages) &&
                AreTheSame(books.Select(b => b.ReleaseDate).ToList(), template.ReleaseDates) &&
                AreTheSame(books.Select(b => b.Genre?.Name).ToList()!, template.GenreNames) &&
                AreTheSame(books.Select(b => b.Author?.Name).ToList()!, template.AuthorNames) &&
                AreTheSame(books.Select(b => b.Publisher?.Name).ToList()!, template.PublisherNames))
            {
                return true;
            }

            return false;
        }

        private bool AreTheSame<T>(List<T>? original, List<T>? template)
        {
            if (original is null || template is null)
            {
                return original == template;
            }

            if (original.Count != template.Count)
            {
                return false;
            }

            List<T> originalSorted = original.OrderBy(t => t).ToList();
            List<T> templateSorted = template.OrderBy(t => t).ToList();

            return originalSorted.SequenceEqual(templateSorted);
        }

        private class TestData
        {
            public static IEnumerable<object[]> GetJsonFileWithCorrectDate()
            {
                string content1 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,1960-07-11,Harper Lee,HarperCollins
                    1984,328,Science Fiction,1949-06-08,George Orwell,Signet Classics
                    """;

                string content2 =
                    """
                    Pages,Title,Genre,ReleaseDate,Author,Publisher
                    336,Kill a Mockingbird,Fiction,1960-07-11,Harper Lee,HarperCollins
                    328,1984,Science Fiction,1949-06-08,George Orwell,Signet Classics
                    """;

                string content3 =
                    """
                    Pages,Title,ReleaseDate,Genre,Author,Publisher
                    336,Kill a Mockingbird,1960-07-11,Fiction,Harper Lee,HarperCollins
                    328,1984,1949-06-08,Science Fiction,George Orwell,Signet Classics
                    """;

                string content4 =
                    """
                    Author,Publisher,ReleaseDate,Genre,Pages,Title
                    Harper Lee,HarperCollins,1960-07-11,Fiction,336,Kill a Mockingbird
                    George Orwell,Signet Classics,1949-06-08,Science Fiction,328,1984
                    """;

                DbTemplate template = 
                    new (Titles: ["Kill a Mockingbird", "1984"],
                    Pages: [336, 328],
                    ReleaseDates: [new DateTime(1960, 7, 11), new DateTime(1949, 6, 8)],
                    GenreNames: ["Fiction", "Science Fiction"],
                    AuthorNames: ["Harper Lee", "George Orwell"],
                    PublisherNames: ["HarperCollins", "Signet Classics"]);

                return new List<object[]>
                {
                    new object[] {content1, template},
                    new object[] {content2, template},
                    new object[] {content3, template},
                    new object[] {content4, template},
                };
            }
        }
    }
}