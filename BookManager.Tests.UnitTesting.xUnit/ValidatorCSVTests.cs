using BookManager.Validators;

namespace BookManager.Tests.UnitTesting.xUnit
{
    public class ValidatorCSVTests
    {
        public class PathValidatorTests
        {
            private ValidatorCSV _validator;

            public PathValidatorTests()
            {
                _validator = new();
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            public void Validate_NullOrEmpty_ThrowsArgumentException(string? path)
            {
                // Act & Assert
                Assert.Throws<ArgumentException>(() => _validator.Validate(path));
            }

            [Theory]
            [InlineData("input")]
            [InlineData("inputcsv")]
            public void Validate_InvalidFileExtension_ThrowsInvalidOperationException(string path)
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => _validator.Validate(path));
            }

            [Theory]
            [InlineData(".csv")]
            [InlineData("unknows.csv")]
            [InlineData("./unknows.csv")]
            public void Validate_InvalidPath_ThrowsFileNotFoundException(string path)
            {
                // Act & Assert
                Assert.Throws<FileNotFoundException>(() => _validator.Validate(path));
            }
        }

        public class DataValidatorTests : IDisposable
        {
            private readonly ValidatorCSV _validator;

            private readonly string _dir;
            private readonly string _file;
            private readonly string _path;

            public DataValidatorTests()
            {
                _validator = new();
                _dir = "DataValidatorTests";
                _file = "Temp.csv";
                _path = $@".\{_dir}\{_file}";

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
            public void Validate_NullHeader_ThrowsArgumentNullException()
            {
                // Arrange
                string content = string.Empty;
                File.WriteAllText(_path, content);

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() => _validator.Validate(_path));
            }

            [Theory]
            [InlineData("Title")]
            [InlineData("Title,Pages,Genre,Author,Publisher")]
            [InlineData("Title,Pages,Genre,ReleaseDate,Author,Publisher,Country")]
            public void Validate_InvalidHeaderColumnCount_ThrowsArgumentOutOfRangeException(string content)
            {
                // Arrange
                File.WriteAllText(_path, content);

                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() => _validator.Validate(_path));
            }

            [Theory]
            [InlineData("Title,Pages,Genre,Author,Publisher,Title")]
            [InlineData("Title,Pages,Genre,ReleaseDate,Author,Pages")]
            public void Validate_DuplicateColumns_ThrowsInvalidDataException(string content)
            {
                // Arrange
                File.WriteAllText(_path, content);

                // Act & Assert
                Assert.Throws<InvalidDataException>(() => _validator.Validate(_path));
            }

            [Theory]
            [MemberData(nameof(TestData.GetContentGeneratedInvalidOperationException), MemberType = typeof(TestData))]
            public void Validate_InvalidContent_ThrowsInvalidOperationException(string content)
            {
                // Arrange
                File.WriteAllText(_path, content);

                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => _validator.Validate(_path));
            }
        }

        private class TestData
        {
            public static IEnumerable<object[]> GetContentGeneratedInvalidOperationException()
            {
                // Invalid header column name
                string content1 =
                    """
                    Title,Pages,UnknownName,ReleaseDate,Author,Publisher    
                    """;

                // Invalid header column name
                string content2 =
                    """
                    ,Pages,Genre,ReleaseDate,Author,Publisher   
                    """;

                // Invalid header column name
                string content3 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,    
                    """;

                // Incorrect content column count
                string content4 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,1960-07-11,Harper Lee
                    """;

                // Empty last line
                string content5 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,1960-07-11,Harper Lee,HarperCollins

                    """;

                // Incorrect pages count
                string content6 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,String,Fiction,1960-07-11,Harper Lee,HarperCollins
                    """;

                // Incorrect pages count
                string content7 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,-336,Fiction,1960-07-11,Harper Lee,HarperCollins
                    """;

                // Incorrect ReleaseDate
                string content8 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,MustBeData,Harper Lee,HarperCollins
                    """;

                // Incorrect ReleaseDate
                string content9 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,-1960-07-11,Harper Lee,HarperCollins
                    """;

                // Title is empty
                string content10 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    ,336,Fiction,1960-07-11,Harper Lee,HarperCollins
                    """;

                // Title is white space
                string content11 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                        ,336,Fiction,1960-07-11,Harper Lee,HarperCollins
                    """;

                // Genre is empty
                string content12 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,,1960-07-11,Harper Lee,HarperCollins
                    """;

                // Genre is white space
                string content13 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,    ,1960-07-11,Harper Lee,HarperCollins
                    """;

                // Author is empty
                string content14 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,1960-07-11,,HarperCollins
                    """;

                // Author is white space
                string content15 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,1960-07-11,    ,HarperCollins
                    """;

                // Publisher is empty
                string content16 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,1960-07-11,Harper Lee,
                    """;

                // Publisher is white space
                string content17 =
                    """
                    Title,Pages,Genre,ReleaseDate,Author,Publisher
                    Kill a Mockingbird,336,Fiction,1960-07-11,Harper Lee,    
                    """;
                

                return new List<object[]>()
                {
                    new object[] { content1 },
                    new object[] { content2 },
                    new object[] { content3 },
                    new object[] { content4 },
                    new object[] { content5 },
                    new object[] { content6 },
                    new object[] { content7 },
                    new object[] { content8 },
                    new object[] { content9 },
                    new object[] { content10 },
                    new object[] { content11 },
                    new object[] { content12 },
                    new object[] { content13 },
                    new object[] { content14 },
                    new object[] { content15 },
                    new object[] { content16 },
                    new object[] { content17 },
                };
            }
        }
    }
}