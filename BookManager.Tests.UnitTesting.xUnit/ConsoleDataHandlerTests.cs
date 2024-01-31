using BookManager.InputData;

namespace BookManager.Tests.UnitTesting.xUnit
{
    [Collection("ConsoleStreamCollection")]
    public class ConsoleDataHandlerTests : IDisposable
    {
        private StringReader? _reader;
        private ConsoleDataHandler _handler;

        public ConsoleDataHandlerTests()
        {
            _handler = new();
        }

        public void Dispose()
        {
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
            _reader?.Dispose();

            GC.SuppressFinalize(this);
        }


        [Fact]
        public void GetPath_NullInput_ReturnNull()
        {
            // Arrange
            string input = string.Empty;
            _reader = new(input);
            Console.SetIn(_reader);

            // Act
            string? actualPath = _handler.GetPath();

            // Assert
            Assert.Null(actualPath);
        }

        [Fact]
        public void GetPath_TextLine_ReturnTheSameTextLine()
        {
            // Arrange
            string input = "Test line";
            _reader = new(input);
            Console.SetIn(_reader);

            // Act
            string? actualPath = _handler.GetPath();

            // Assert
            Assert.Equal(input, actualPath);
        }
    }
}
