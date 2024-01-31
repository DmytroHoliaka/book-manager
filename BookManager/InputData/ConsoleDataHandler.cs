namespace BookManager.InputData
{
    public class ConsoleDataHandler : IDataHandler
    {
        public string? GetPath()
        {
            Console.Write("Enter path to input file: ");
            string? path = Console.ReadLine();
            return path;
        }
    }
}
