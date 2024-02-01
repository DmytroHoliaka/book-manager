using BookManager.Database;
using BookManager.Validators;

namespace BookManager.FileProcessing
{
    public class FileParserCSV : IFileParser
    {   
        public IDatabaseManager DatabaseManager { get; }
        public List<ColumnName> FileColumnOrder =>
            _fileColumnOrder.ConvertAll(name => name);

        private readonly List<ColumnName> _fileColumnOrder;

        public FileParserCSV(IDatabaseManager databaseManager)
        {
            DatabaseManager = databaseManager;
            _fileColumnOrder = [];
        }

        public async Task ParseAsync(string pathCSV)
        {
            ValidatorCSV validator = new();
            validator.Validate(pathCSV);

            using StreamReader reader = new(pathCSV);
            ProcessHeaderCSV(reader);
            await FillDatabase(reader);
        }

        private void ProcessHeaderCSV(StreamReader reader)
        {
            string line = reader.ReadLine()!;
            string[] columnNames = line.Split(',');

            foreach (string column in columnNames)
            {
                ColumnName header = (ColumnName)Enum.Parse(typeof(ColumnName), column);
                _fileColumnOrder.Add(header);
            }
        }

        private async Task FillDatabase(StreamReader reader)
        {
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] columnNames = line.Split(',');

                Guid genreId = await ProcessGenreAsync(columnNames);
                Guid authorId = await ProcessAuthorAsync(columnNames);
                Guid publisherId = await ProcessPublisherAsync(columnNames);

                await ProcessBook(columnNames, genreId, authorId, publisherId);
            }
        }

        private async Task<Guid> ProcessGenreAsync(string[] columns)
        {
            int index = _fileColumnOrder.IndexOf(ColumnName.Genre);
            string genreName = columns[index];

            Guid genreId = await DatabaseManager.EnsureInsertedGenreAsync(genreName);
            return genreId;
        }

        private async Task<Guid> ProcessAuthorAsync(string[] columns)
        {
            int index = _fileColumnOrder.IndexOf(ColumnName.Author);
            string authorName = columns[index];

            Guid authorId = await DatabaseManager.EnsureInsertedAuthorAsync(authorName);
            return authorId;
        }

        private async Task<Guid> ProcessPublisherAsync(string[] columns)
        {
            int index = _fileColumnOrder.IndexOf(ColumnName.Publisher);
            string publisherName = columns[index];

            Guid publisherId = await DatabaseManager.EnsureInsertedPublisherAsync(publisherName);
            return publisherId;
        }

        private async Task ProcessBook(string[] columns, Guid genreId, Guid? authorId, Guid? publisherId)
        {
            int indexTitle = _fileColumnOrder.IndexOf(ColumnName.Title);
            int indexPages = _fileColumnOrder.IndexOf(ColumnName.Pages);
            int indexReleaseDate = _fileColumnOrder.IndexOf(ColumnName.ReleaseDate);

            string title = columns[indexTitle];
            string pages = columns[indexPages];

            string? releaseDateStr = columns[indexReleaseDate] == string.Empty ? null : columns[indexReleaseDate];
            DateTime? releaseDate = releaseDateStr is null ? null : Convert.ToDateTime(releaseDateStr);

            await DatabaseManager.EnsureInsertedBookAsync(title, Convert.ToInt32(pages), releaseDate, genreId, authorId, publisherId);
        }
    }
}
