using BookManager.Entities;
using BookManager.FileProcessing;
using BookManager.Filtering;
using Moq;

namespace BookManager.Tests.UnitTesting.xUnit
{
    public abstract class OutputDispatcherTests
    {
        public OutputDispatcher? Outputter { get; set; }
    }

    [Collection("ConsoleStreamCollection")]
    public class ConsoleOutputTests : OutputDispatcherTests, IDisposable
    {
        private readonly StringWriter _writer;

        public ConsoleOutputTests()
        {
            _writer = new();
            Console.SetOut(_writer);
        }

        public void Dispose()
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));
            _writer.Dispose();

            GC.SuppressFinalize(this);
        }

        [Theory]
        [MemberData(nameof(GetConsoleContent))]
        public void OutputToConsole_Books_ReturnsBookTitles(IEnumerable<Book> books,
                                                            List<ColumnName> order,
                                                            string expected)
        {
            // Arrange
            Outputter = new(books, order);

            // Act
            Outputter.OutputToConsole();

            // Assert
            Assert.Equal(expected, _writer.ToString());
        }

        public static IEnumerable<object[]> GetConsoleContent()
        {
            Author alex = new() { Name = "Alex" };
            Genre fantasy = new() { Name = "Fantacy" };
            Publisher scholastic = new() { Name = "Scholastic" };

            Book hobit = new()
            {
                Title = "The Hobbit",
                Pages = 176,
                ReleaseDate = new DateTime(1947, 2, 1),
                Genre = fantasy,
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
                Publisher = scholastic,
            };

            IEnumerable<Book> books1 = [];
            IEnumerable<Book> books2 = [hobit];
            IEnumerable<Book> books3 = [hobit, animalFarm];

            string expected1 =
                "Total count: 0" +
                Environment.NewLine +
                Environment.NewLine;

            string expected2 =
                "Total count: 1" +
                Environment.NewLine +
                Environment.NewLine +
                "The Hobbit" +
                Environment.NewLine;

            string expected3 =
                "Total count: 2" +
                Environment.NewLine +
                Environment.NewLine +
                "The Hobbit" +
                Environment.NewLine +
                "Animal Farm" +
                Environment.NewLine;


            List<ColumnName> order =
            [
                ColumnName.Title,
                ColumnName.Pages,
                ColumnName.Genre,
                ColumnName.ReleaseDate,
                ColumnName.Author,
                ColumnName.Publisher
            ];


            return new List<object[]>()
            {
                new object[] {books1, order, expected1},
                new object[] {books2, order, expected2},
                new object[] {books3, order, expected3},
            };
        }
    }

    public class FileOutputTests : OutputDispatcherTests, IDisposable
    {
        private readonly string _dir;
        private readonly string _file;
        private readonly string _path;

        public FileOutputTests()
        {
            _dir = "FileOutputTests";
            _file = "Temp.csv";
            _path = @$".\{_dir}\{_file}";

            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_dir))
            {
                Directory.Delete(_dir, true);
            }

            GC.SuppressFinalize(this);
        }

        [Fact]
        public void OutputToCSV_Null_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<Book> books = new List<Book>();

            List<ColumnName> order =
            [
                ColumnName.Title,
                ColumnName.Pages,
                ColumnName.Genre,
                ColumnName.ReleaseDate,
                ColumnName.Author,
                ColumnName.Publisher
            ];

            Outputter = new(books, order);
            string? path = null;

            // Act & Arrange
            Assert.Throws<ArgumentNullException>(() => Outputter.OutputToCSV(path));
        }

        [Theory]
        [MemberData(nameof(GetFileContent))]
        public void OutputToCSV_Books_ReturnsFileWithBookDetails(IEnumerable<Book> books,
                                                                 List<ColumnName> order,
                                                                 string expected)
        {
            // Arrange
            Outputter = new(books, order);

            // Act
            Outputter.OutputToCSV(_path);

            // Assert
            string actual = File.ReadAllText(_path);
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> GetFileContent()
        {
            Author alex = new() { Name = "Alex" };
            Genre fantasy = new() { Name = "Fantasy" };
            Publisher scholastic = new() { Name = "Scholastic" };

            Book hobit = new()
            {
                Title = "The Hobbit",
                Pages = 176,
                ReleaseDate = new DateTime(1947, 2, 1),
                Genre = fantasy,
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
                Publisher = scholastic,
            };

            IEnumerable<Book> books1 = [];
            IEnumerable<Book> books2 = [hobit];
            IEnumerable<Book> books3 = [hobit, animalFarm];

            List<ColumnName> order1 =
            [
                ColumnName.Title,
                ColumnName.Pages,
                ColumnName.Genre,
                ColumnName.ReleaseDate,
                ColumnName.Author,
                ColumnName.Publisher
            ];

            List<ColumnName> order2 =
            [
                ColumnName.Pages,
                ColumnName.Title,
                ColumnName.Author,
                ColumnName.ReleaseDate,
                ColumnName.Genre,
                ColumnName.Publisher
            ];


            string expected11 =
                "Title,Pages,Genre,ReleaseDate,Author,Publisher" +
                Environment.NewLine;

            string expected12 =
                "Pages,Title,Author,ReleaseDate,Genre,Publisher" +
                Environment.NewLine;

            string expected21 =
                "Title,Pages,Genre,ReleaseDate,Author,Publisher" +
                Environment.NewLine +
                "The Hobbit,176,Fantasy,1947-02-01,Alex,Scholastic" +
                Environment.NewLine;

            string expected22 =
                "Pages,Title,Author,ReleaseDate,Genre,Publisher" +
                Environment.NewLine +
                "176,The Hobbit,Alex,1947-02-01,Fantasy,Scholastic" +
                Environment.NewLine;

            string expected31 =
                "Title,Pages,Genre,ReleaseDate,Author,Publisher" +
                Environment.NewLine +
                "The Hobbit,176,Fantasy,1947-02-01,Alex,Scholastic" +
                Environment.NewLine +
                "Animal Farm,324,Fantasy,1995-05-06,Alex,Scholastic" +
                Environment.NewLine;

            string expected32 =
                "Pages,Title,Author,ReleaseDate,Genre,Publisher" +
                Environment.NewLine +
                "176,The Hobbit,Alex,1947-02-01,Fantasy,Scholastic" +
                Environment.NewLine +
                "324,Animal Farm,Alex,1995-05-06,Fantasy,Scholastic" +
                Environment.NewLine;

            return new List<object[]>()
            {
                new object[] {books1, order1, expected11},
                new object[] {books1, order2, expected12},
                new object[] {books2, order1, expected21},
                new object[] {books2, order2, expected22},
                new object[] {books3, order1, expected31},
                new object[] {books3, order2, expected32},
            };
        }
    }

    public class OutputDispatcherStaticTests
    {
        [Fact]
        public void GetOutputFileName_DateTime_ReturnsUniqueFileName()
        {
            // Arrange
            Mock<ITimeProvider> mock = new();
            mock.Setup(d => d.Now).Returns(new DateTime(2020, 12, 30, 18, 30, 55, 175));
            string expected = $"../../../../Outputs/output (2020.12.30 18-30-55.1750).csv";

            // Act
            string actual = OutputDispatcher.GetOutputFileName(mock.Object);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void EnsureDirectoryExists_Null_ThrowsArgumentNullException()
        {
            // Arrange
            string? path = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => OutputDispatcher.EnsureDirectoryCreated(path));
        }

        [Fact]
        public void EnsureDirectoryExists_NullDirectory_ArgumentNullException()
        {
            // Arrange
            string? path = DriveInfo.GetDrives()[0].Name;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => OutputDispatcher.EnsureDirectoryCreated(path));
        }

        [Fact]
        public void EnsureDirectoryExists_FilePathWithoutDirectory_CreatesEmptyDirectory()
        {
            // Arrange
            string filePath = @".\IndependentTests\temp.csv";
            string dirPath = @".\IndependentTests";

            // Act 
            OutputDispatcher.EnsureDirectoryCreated(filePath);

            // Assert
            Assert.True(Directory.Exists(dirPath));

            // Clean up
            Directory.Delete(dirPath, true);
        }

        [Fact]
        public void EnsureDirectoryExists_FilePathWithDirectory_IgnoresCreatingDirectory()
        {
            // Arrange
            string filePath = @".\IndependentTests\temp.csv";
            string dirPath = @".\IndependentTests";

            Directory.CreateDirectory(dirPath);

            // Act 
            OutputDispatcher.EnsureDirectoryCreated(filePath);

            // Assert
            Assert.True(Directory.Exists(dirPath));

            // Clean up
            Directory.Delete(dirPath, true);
        }
    }
}
