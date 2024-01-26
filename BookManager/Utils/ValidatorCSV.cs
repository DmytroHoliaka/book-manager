using BookManager.FileProcessing;

namespace BookManager.Utils
{
    public class ValidatorCSV : IFileValidator
    {
        public IEnumerable<ParseError> Errors =>
            _errors.Select(err => new ParseError(err));

        private readonly List<ParseError> _errors;
        private const int _columnCount = 6;
        private int _pagesIndex;
        private int _dateIndex;


        public ValidatorCSV()
        {
            _errors = [];
        }

        public void Validate(string pathCSV)
        {
            ValidateExtention(pathCSV, ".csv");

            using StreamReader reader = new(pathCSV);
            string[] columnNames = ValidateHeader(reader);

            InitializeIndexes(columnNames);

            ValidateContent(reader);
        }

        private static void ValidateExtention(string path, string extention)
        {
            FileInfo file = new(path);

            if (file.Extension != extention)
            {
                throw new InvalidOperationException("File have incorrect extention");
            }
        }

        private string[] ValidateHeader(StreamReader reader)
        {
            string? header = reader.ReadLine();

            if (header is null)
            {
                throw new ArgumentNullException(nameof(header), "Header in file can't be null");
            }

            string[] columnNames = header.Split(',');

            if (columnNames.Length != _columnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(columnNames), "Incorrect count of columns");
            }

            foreach (string columnName in columnNames)
            {
                if (Enum.IsDefined(typeof(ColumnName), columnName) == false)
                {
                    _errors.Add(new ParseError(1, header, $"Incorrect column name", columnName));
                }
            }

            if (_errors.Any())
            {
                throw new InvalidOperationException(ParseError.GetErrors(_errors));
            }

            return columnNames;
        }

        private void InitializeIndexes(string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; ++i)
            {
                if ((ColumnName)Enum.Parse(typeof(ColumnName), columnNames[i]) == ColumnName.Pages)
                {
                    _pagesIndex = i;
                }
                else if ((ColumnName)Enum.Parse(typeof(ColumnName), columnNames[i]) == ColumnName.ReleaseDate)
                {
                    _dateIndex = i;
                }
            }
        }

        private void ValidateContent(StreamReader reader)
        {
            string? line;
            int lineCount = 1;

            while ((line = reader.ReadLine()) != null)
            {
                string[] columnNames = line.Split(',');
                lineCount += 1;

                if (columnNames.Length != _columnCount)
                {
                    _errors.Add(new ParseError(lineCount, line, "Incorrect data amount in line", $"{columnNames.Length}, correct {_columnCount}"));
                    continue;
                }

                if (IsCorrectPages(columnNames[_pagesIndex]) == false)
                {
                    _errors.Add(new ParseError(lineCount, line, "Incorrect pages count", columnNames[_pagesIndex]));
                }

                if (IsCorrectDate(columnNames[_dateIndex]) == false)
                {
                    _errors.Add(new ParseError(lineCount, line, "Incorrect date format", columnNames[_dateIndex]));
                }
            }

            if (IsFileEndsEmptyLine(reader))
            {
                lineCount += 1;
                _errors.Add(new ParseError(lineCount, string.Empty, "Last line is empty", "All columns"));
            }

            if (_errors.Any())
            {
                throw new InvalidOperationException(ParseError.GetErrors(_errors));
            }
        }

        private static bool IsFileEndsEmptyLine(StreamReader reader)
        {
            reader.BaseStream.Seek(-1, SeekOrigin.End);

            int newLineCharCode = 10;
            int lastCharCode = reader.Read();

            return lastCharCode == newLineCharCode;
        }

        private static bool IsCorrectDate(string dateLine)
        {
            return DateTime.TryParse(dateLine, out DateTime _);
        }

        private static bool IsCorrectPages(string pagesLine)
        {
            bool check = int.TryParse(pagesLine, out int pages);

            if (check == false || pages <= 0)
            {
                return false;
            }

            return true;
        }
    }
}
