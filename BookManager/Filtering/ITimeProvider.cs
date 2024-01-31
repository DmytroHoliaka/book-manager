namespace BookManager.Filtering
{
    public interface ITimeProvider
    {
        public DateTime Now { get; }
    }
}
