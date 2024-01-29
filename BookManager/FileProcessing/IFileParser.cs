using BookManager.Database;

namespace BookManager.FileProcessing
{
    public interface IFileParser
    {
        IDatabaseManager DatabaseManager { get; }

        Task ParseAsync(string pathCSV);
    }
}
