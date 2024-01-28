using System.Text;

namespace BookManager.Validators
{
    public class ParseError
    {
        private readonly int _line;
        private readonly string _content;
        private readonly string _text;
        private readonly string _value;

        public ParseError(int line, string content, string text, string columnError)
        {
            _line = line;
            _content = content;
            _text = text;
            _value = columnError;
        }

        public ParseError(ParseError origin)
        {
            _line = origin._line;
            _content = origin._content;
            _text = origin._text;
            _value = origin._value;
        }

        public override string ToString()
        {
            return $"Line: {_line}  ({_content})  {_text} ({_value})";
        }

        public static string GetErrors(List<ParseError> errors)
        {
            StringBuilder msg = new();

            foreach (ParseError error in errors)
            {
                msg.Append(error.ToString() + '\n');
            }

            return msg.ToString();
        }
    }
}
