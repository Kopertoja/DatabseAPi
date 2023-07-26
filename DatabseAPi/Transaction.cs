namespace DatabseAPi
{
    public class Transaction
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
    }

}
