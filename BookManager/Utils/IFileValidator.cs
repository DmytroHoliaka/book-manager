namespace BookManager.Utils
{
    public interface IFileValidator
    {
        IEnumerable<ParseError> Errors { get; }

        void Validate(string path);
    }
}
