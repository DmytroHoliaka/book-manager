namespace BookManager.Validators
{
    public class ValidatorInputData
    {
        public static void ValidateFilePath(string? path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path), "Input path cannot be null");
            }

            if (File.Exists(path) == false)
            {
                throw new FileNotFoundException("File doesn't exists", nameof(path));
            }
        }
    }
}
