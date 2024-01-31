using BookManager.Entities;
using BookManager.FileProcessing;

namespace BookManager.Filtering
{
    public class OutputDispatcher
    {
        private readonly IEnumerable<Book> _books;
        private readonly List<ColumnName> _columnOrder;

        public OutputDispatcher(IEnumerable<Book>? books, List<ColumnName>? columnOrder)
        {
            if (books is null)
            {
                throw new ArgumentNullException(nameof(books), "Books cannot be null");
            }

            if (columnOrder is null)
            {
                throw new ArgumentNullException(nameof(columnOrder), "Column order cannot be null");
            }

            if (columnOrder.Count != 6)
            {
                throw new ArgumentOutOfRangeException(nameof(columnOrder), "Incorrect count of columns");
            }

            _books = books;
            _columnOrder = columnOrder;
        }

        public void OutputToCSV(string? pathCSV)
        {
            if (pathCSV is null)
            {
                throw new ArgumentNullException(nameof(pathCSV), "Output file cannot be null");
            }

            Dictionary<ColumnName, string?> outputDate = [];
            using StreamWriter writer = new(pathCSV);

            writer.WriteLine(string.Join(',', _columnOrder));

            foreach (Book book in _books)
            {
                outputDate[ColumnName.Title] = book.Title!;
                outputDate[ColumnName.Pages] = book.Pages.ToString();
                outputDate[ColumnName.Genre] = book.Genre?.Name!;
                outputDate[ColumnName.ReleaseDate] = book.ReleaseDate?.ToString("yyyy-MM-dd");
                outputDate[ColumnName.Author] = book.Author?.Name;
                outputDate[ColumnName.Publisher] = book.Publisher?.Name;

                writer.WriteLine($"{outputDate[_columnOrder[0]]}," +
                                 $"{outputDate[_columnOrder[1]]}," +
                                 $"{outputDate[_columnOrder[2]]}," +
                                 $"{outputDate[_columnOrder[3]]}," +
                                 $"{outputDate[_columnOrder[4]]}," +
                                 $"{outputDate[_columnOrder[5]]}");
            }
        }

        public void OutputToConsole()
        {
            Console.WriteLine($"Total count: {_books.Count()}");
            Console.WriteLine();

            foreach (string? title in _books.Select(b => b.Title).Distinct())
            {
                Console.WriteLine(title);
            }
        }

        public static string GetOutputFileName(ITimeProvider time)
        {
            string pathCSV = $"../../../../Outputs/output ({time.Now:yyyy.MM.dd HH-mm-ss.ffff}).csv";
            return pathCSV;
        }

        public static void EnsureDirectoryCreated(string? filePath)
        {
            if (filePath is null)
            {
                throw new ArgumentNullException(nameof(filePath), "File path cannot be null");
            }

            FileInfo file = new(filePath);
            DirectoryInfo? dir = file.Directory;

            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir), "Can't create directory for output file");
            }

            if (dir.Exists == false)
            {
                dir.Create();
            }
        }
    }
}
