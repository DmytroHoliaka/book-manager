namespace BookManager.Filtering
{
    public class SystemTimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
