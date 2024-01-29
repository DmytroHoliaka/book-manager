using BookManager.Configuration;
using BookManager.Entities;
using BookManager.Filtering;

namespace BookManager.Tests.UnitTesting.xUnit
{
    public class ConfiguratorJsonTests : IDisposable
    {
        private readonly string _dirName;
        private readonly string _fileName;

        public ConfiguratorJsonTests()
        {
            _dirName = @".\ConfiguratorJsonTests";
            _fileName = @".\Temp.csv";

            Directory.CreateDirectory(_dirName);
        }

        public void Dispose()
        {
            if (Directory.Exists(_dirName))
            {
                Directory.Delete(_dirName, true);
            }

            GC.SuppressFinalize(this);
        }

        [Theory]
        [MemberData(nameof(TestData.GetDataForCorrectJsonFileTest), MemberType = typeof(TestData))]
        public void GetFilter_CorrectJsonFile_ReturnsFilterInstance(string section, string content, FilterTemplate template)
        {
            // Arrange
            WriteIntoFile(content, _fileName);
            ConfiguratorJson config = new(_fileName, _dirName);

            // Act
            Filter filter = config.GetFilter(section);

            // Assert
            Assert.True(IsInstancesMatch(filter, template));
        }

        [Theory]
        [MemberData(nameof(TestData.GetDataForIncorrectJsonFileTest), MemberType = typeof(TestData))]
        public void GetFilter_IncorrectJsonFile_ThrowsInvalidOperationException(string section, string content)
        {
            // Arrange
            WriteIntoFile(content, _fileName);
            ConfiguratorJson config = new(_fileName, _dirName);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => config.GetFilter(section));
        }

        [Theory]
        [MemberData(nameof(TestData.GetDataFromNullCheck), MemberType = typeof(TestData))]
        public void GetFilter_IncorrectJsonFile_ThrowsInvalidDataException(string section, string content)
        {
            // Arrange
            WriteIntoFile(content, _fileName);
            ConfiguratorJson config = new(_fileName, _dirName);

            // Act & Assert
            Assert.Throws<InvalidDataException>(() => config.GetFilter(section));
        }

        public record FilterTemplate(string Title,
                                     string Genre,
                                     string Author,
                                     string Publisher,
                                     int? MoreThanPages,
                                     int? LessThanPages,
                                     DateTime? PublishedBefore,
                                     DateTime? PublishedAfter);


        private static bool IsInstancesMatch(Filter original, FilterTemplate template)
        {
            return
                original.Title == template.Title &&
                original.Genre == template.Genre &&
                original.Author == template.Author &&
                original.Publisher == template.Publisher &&
                original.MoreThanPages == template.MoreThanPages &&
                original.LessThanPages == template.LessThanPages &&
                original.PublishedBefore == template.PublishedBefore &&
                original.PublishedAfter == template.PublishedAfter;
        }

        private void WriteIntoFile(string content, string jsonName)
        {
            string filePath = $@"{_dirName}\{jsonName}";
            File.WriteAllText(filePath, content);
        }

        private class TestData
        {
            public static IEnumerable<object[]> GetDataForCorrectJsonFileTest()
            {
                string sectionName = "Filters";

                string content1 =
                    """
                    {
                      "Filters": {
                        "Title": "",
                        "Genre": "",
                        "Author": "",
                        "Publisher": "",
                        "MoreThanPages": "",
                        "LessThanPages": "",
                        "PublishedBefore": "",
                        "PublishedAfter": ""
                      }
                    }
                    """;

                string content2 =
                    """
                    {
                      "Filters": {
                        "Title": "The Old Man and the Sea",
                        "Genre": "Fiction",
                        "Author": "Ernest Hemingway",
                        "Publisher": "Charles Scribner's Sons",
                        "MoreThanPages": "",
                        "LessThanPages": "128",
                        "PublishedBefore": "1953-09-01",
                        "PublishedAfter": "1951-09-01"
                      }
                    }
                    """;

                FilterTemplate template1 = new(Title: "",
                                               Genre: "",
                                               Author: "",
                                               Publisher: "",
                                               MoreThanPages: null,
                                               LessThanPages: null,
                                               PublishedBefore: null,
                                               PublishedAfter: null);

                FilterTemplate template2 = new(Title: "The Old Man and the Sea",
                                               Genre: "Fiction",
                                               Author: "Ernest Hemingway",
                                               Publisher: "Charles Scribner's Sons",
                                               MoreThanPages: null,
                                               LessThanPages: 128,
                                               PublishedBefore: new DateTime(1953, 9, 1),
                                               PublishedAfter: new DateTime(1951, 9, 1));

                return new List<object[]>()
                {
                    new object[] {sectionName, content1, template1},
                    new object[] {sectionName, content2, template2},
                };
            }


            public static IEnumerable<object[]> GetDataForIncorrectJsonFileTest()
            {
                string sectionName = "Filters";

                // PublishedBefore incorrect value
                string content1 =
                    """
                    {
                      "Filters": {
                        "Title": "",
                        "Genre": "",
                        "Publisher": "",
                        "MoreThanPages": "",
                        "LessThanPages": "",
                        "PublishedBefore": "-2000-01-01",
                        "PublishedAfter": ""
                      }
                    }
                    """;

                // MoreThatPages incorrect value
                string content2 =
                    """
                    {
                      "Filters": {
                        "Title": "The Old Man and the Sea",
                        "Genre": "Fiction",
                        "Author": "Ernest Hemingway",
                        "Publisher": "Charles Scribner's Sons",
                        "MoreThanPages": "must be number",
                        "LessThanPages": "128",
                        "PublishedBefore": "1953-09-01",
                        "PublishedAfter": "1951-09-01"
                      }
                    }
                    """;

                return new List<object[]>()
                {
                    new object[] {sectionName, content1},
                    new object[] {sectionName, content2},
                };
            }

            public static IEnumerable<object[]> GetDataFromNullCheck()
            {
                string sectionName = "Filters";

                // Wrong section name
                string content =
                    """
                    {
                      "Temp": {
                        "Title": "",
                        "Genre": "",
                        "Publisher": "",
                        "MoreThanPages": "",
                        "LessThanPages": "",
                        "PublishedBefore": "-2000-01-01",
                        "PublishedAfter": ""
                      }
                    }
                    """;

                return new List<object[]>()
                {
                    new object[] {sectionName, content},
                };
            }


        }
    }
}