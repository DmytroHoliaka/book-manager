using BookManager.Configuration;
using BookManager.Database;
using BookManager.Entities;
using BookManager.FileProcessing;
using BookManager.Filtering;
using BookManager.Validators;
using Microsoft.Extensions.Configuration;

namespace BookManager.Execution
{
    public class Facade
    {
        public static async Task Execute(string? path)
        {
            ValidatorInputData.ValidateFilePath(path);

            string jsonName = "appSettings.json";
            string sectionName = "Filters";
            string pathCSV = OutputDispatcher.GetOutputFileName(new SystemTimeProvider());
            OutputDispatcher.EnsureDirectoryCreated(pathCSV);

            ConfiguratorJson configManager = new(jsonName);
            configManager!.TrimSectionValues(sectionName);

            IConfiguration config = configManager.Config;
            List<ColumnName>? columnOrder;

            
            using ApplicationContext context = new(config);

            IDatabaseManager databaseManager = new DatabaseManager(context);

            FileParserCSV parser = new(databaseManager);
            await parser.ParseAsync(path!);
            columnOrder = parser.FileColumnOrder;

            Filter filter = configManager.GetFilter(sectionName);
            Searcher searcher = new(context);
            List<Book> filteredBooks = await searcher.GetFilteredBooks(filter);

            OutputDispatcher outputer = new(filteredBooks, columnOrder);
            outputer.OutputToConsole();
            outputer.OutputToCSV(pathCSV);

        }
    }
}
