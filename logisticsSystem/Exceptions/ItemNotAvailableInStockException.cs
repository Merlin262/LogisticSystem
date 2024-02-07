namespace logisticsSystem.Exceptions
{
    public class ItemNotAvailableInStockException : Exception
    {
        public ItemNotAvailableInStockException(string message) : base(message)
        {
        }
    }
}
