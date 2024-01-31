namespace BookManager.Validators
{
    public interface IFileValidator
    {
        IEnumerable<ParseError> Errors { get; }

        void Validate(string path);
    }
}
