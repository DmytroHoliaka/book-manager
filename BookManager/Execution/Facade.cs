using BookManager.Configuration;
using BookManager.Database;
using BookManager.FileProcessing;
using BookManager.Filtering;
using BookManager.Validators;
using Microsoft.Extensions.Configuration;

namespace BookManager.Execution
{
    public class Facade
    {
        public static void Execute(string? path)
        {
            ValidatorInputData.ValidateFilePath(path);

            string jsonName = "appSettings.json";
            string sectionName = "Filters";
            string pathCSV = OutputDispatcher.GetOutputFileName();

            IConfigurator configManager = new ConfiguratorJson(jsonName);
            IConfiguration config = configManager.Config;

            using (ApplicationContext context = new(config))
            {
                IDatabaseManager databaseManager = new DatabaseManager(context);

                IFileParser parser = new FileParserCSV(databaseManager);
                parser.Parse(path!);

                Filter filter = configManager.GetFilter(sectionName);
                Searcher searcher = new(filter, context);

                OutputDispatcher outputer = new(searcher.FillteredBook, (parser as FileParserCSV)?.FileColumnOrder);
                outputer.OutputToConsole();
                outputer.OutputToCSV(pathCSV);
            }
        }
    }
}
