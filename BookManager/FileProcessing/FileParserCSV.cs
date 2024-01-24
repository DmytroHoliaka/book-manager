using BookManager.Context;

namespace BookManager.FileProcessing
{
    public class FileParserCSV : IFileParser
    {
        public IDatabaseManager DatabaseManager { get; }
        private readonly IList<ColumnName> _fileColumnOrder;

        public FileParserCSV()
        {
            DatabaseManager = new DatabaseManager();
            _fileColumnOrder = new List<ColumnName>();
        }

        public void Parse(string pathCSV)
        {
            // ToDo: Validator.ValidateFile();

            using (StreamReader reader = new(pathCSV))
            {
                ProcessHeaderCSV(reader);
                FillDatabase(reader);
            }
        }

        private void ProcessHeaderCSV(StreamReader reader)
        {
            string line = reader.ReadLine()!;
            string[] columnNames = line.Split(',');

            // ToDo: ValidatorCSV.ValidateColumnName() Count = 6

            foreach (string column in columnNames)
            {
                bool check = Enum.TryParse(column, out ColumnName header);

                if (check == false)
                {
                    throw new InvalidOperationException("Incorrect header name");
                }
                else
                {
                    _fileColumnOrder.Add(header);
                }
            }
        }

        private void FillDatabase(StreamReader reader)
        {
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] columnNames = line.Split(',');

                int genreId = ProcessGenre(columnNames);
                int authorId = ProcessAuthor(columnNames);
                int publisherId = ProcessPublisher(columnNames);
                
                ProcessBook(columnNames, genreId, authorId, publisherId);    
            }
        }

        private int ProcessGenre(string[] columns)
        {
            int index = _fileColumnOrder.IndexOf(ColumnName.Genre);
            string genreName = columns[index];

            int genreId = DatabaseManager.InsertGenre(genreName);
            return genreId;
        }

        private int ProcessAuthor(string[] columns)
        {
            int index = _fileColumnOrder.IndexOf(ColumnName.Author);
            string authorName = columns[index];

            int authorId = DatabaseManager.InsertAuthor(authorName);
            return authorId;
        }

        private int ProcessPublisher(string[] columns)
        {
            int index = _fileColumnOrder.IndexOf(ColumnName.Publisher);
            string publisherName = columns[index];

            int publisherId = DatabaseManager.InsertPublisher(publisherName);
            return publisherId;
        }

        private void ProcessBook(string[] columns, int genreId, int authorId, int publisherId)
        {
            int indexTitle = _fileColumnOrder.IndexOf(ColumnName.Title);
            int indexPages = _fileColumnOrder.IndexOf(ColumnName.Pages);
            int indexReleaseDate = _fileColumnOrder.IndexOf(ColumnName.ReleaseDate);

            string title = columns[indexTitle];
            string pages = columns[indexPages];
            string releaseDate = columns[indexReleaseDate];

            // ToDo: Add completely convert validation
            // DatabaseManager.InsertBook(title, Convert.ToInt32(pages), Convert.ToDateTime(releaseDate), genreId, authorId, publisherId);
            // ToDo: Error convert DateTime «8th century DC»
            DatabaseManager.InsertBook(title, Convert.ToInt32(pages), new DateTime(2022, 10, 27), genreId, authorId, publisherId);
        }
    }
}
