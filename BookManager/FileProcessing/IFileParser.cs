using BookManager.Database;

namespace BookManager.FileProcessing
{
    public interface IFileParser
    {
        IDatabaseManager DatabaseManager { get; }

        void Parse(string pathCSV);
    }
}
