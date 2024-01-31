using BookManager.InputData;

namespace BookManager.Execution
{
    public class Program
    {
        // ToDo: Make amendment from mentor's conversation
        public static async Task Main()
        {
            try
            {
                ConsoleDataHandler data = new();
                string? path = data.GetPath();

                await Facade.Execute(path);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("[InvalidOperationException]");
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"[ArgumentNullException] {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[ArgumentOutOfRangeException] {ex.Message}");
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"[InvalidDataException] {ex.Message}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[FileNotFoundException] {ex.Message}");
            }
        }

    }
}
