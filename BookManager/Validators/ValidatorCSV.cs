using BookManager.FileProcessing;

namespace BookManager.Validators
{
    public class ValidatorCSV : IFileValidator
    {
        public IEnumerable<ParseError> Errors =>
            _errors.Select(err => new ParseError(err));

        private readonly List<ParseError> _errors;
        private readonly List<ColumnName> _columnOrder;
        private const int _columnCount = 6;


        public ValidatorCSV()
        {
            _errors = [];
            _columnOrder = [];
        }

        public void Validate(string pathCSV)
        {
            ValidateExtention(pathCSV, ".csv");

            using StreamReader reader = new(pathCSV);
            string[] columnNames = ValidateHeader(reader);

            InitializeColumnOrder(columnNames);

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

        private static bool IsCorrectName(string columnName)
        {
            return string.IsNullOrWhiteSpace(columnName) == false;
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
            return dateLine == string.Empty || DateTime.TryParse(dateLine, out DateTime _);
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


        private string[] ValidateHeader(StreamReader reader)
        {
            string? header = reader.ReadLine();
            int lineCount = 1;

            if (header is null)
            {
                throw new ArgumentNullException(nameof(header), "Header in file can't be null");
            }

            string[] columnNames = header.Split(',');

            if (columnNames.Length != _columnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(columnNames), "Incorrect count of columns");
            }

            if (columnNames.Distinct().Count() != columnNames.Length)
            {
                throw new InvalidDataException("Header have duplicate names");
            }

            foreach (string columnName in columnNames)
            {
                if (Enum.IsDefined(typeof(ColumnName), columnName) == false)
                {
                    _errors.Add(new ParseError(lineCount, header, $"Incorrect column name", columnName));
                }
            }

            if (_errors.Any())
            {
                throw new InvalidOperationException(ParseError.GetErrors(_errors));
            }

            return columnNames;
        }

        private void InitializeColumnOrder(string[] columnNames)
        {
            for (int i = 0; i < columnNames.Length; ++i)
            {
                _columnOrder.Add((ColumnName)Enum.Parse(typeof(ColumnName), columnNames[i]));
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

                ValidateColumns(columnNames, line, lineCount);
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

        private void ValidateColumns(string[] columns, string line, int lineCount)
        {
            for (int i = 0; i < _columnOrder.Count; ++i)
            {
                switch (_columnOrder[i])
                {
                    case ColumnName.Pages:

                        if (IsCorrectPages(columns[i]) == false)
                        {
                            _errors.Add(new ParseError(lineCount, line, "Incorrect pages count", columns[i]));
                        }

                        break;


                    case ColumnName.ReleaseDate:

                        if (IsCorrectDate(columns[i]) == false)
                        {
                            _errors.Add(new ParseError(lineCount, line, "Incorrect date format", columns[i]));
                        }

                        break;


                    case ColumnName.Title:
                    case ColumnName.Genre:
                    case ColumnName.Author:
                    case ColumnName.Publisher:

                        if (IsCorrectName(columns[i]) == false)
                        {
                            _errors.Add(new ParseError(lineCount, line, $"Incorrect {_columnOrder[i]}", columns[i]));
                        }

                        break;


                    default:
                        throw new InvalidDataException("Incorrect column name");
                }
            }
        }
    }
}
