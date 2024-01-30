using BookManager.Validators;

namespace BookManager.Tests.UnitTesting.xUnit
{
    public class ValidatorInputDataTests
    {
        [Fact]
        public void ValidateFilePath_Null_ThrowsArgumentNullException()
        {
            // Arrange
            string? path = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ValidatorInputData.ValidateFilePath(path));
        }

        [Fact]
        public void ValidateFilePath_NotExistingFile_ThrowsFileNotFoundException()
        {
            // Arrange
            string? path = @".\Unknown path";

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => ValidatorInputData.ValidateFilePath(path));
        }
    }
}
