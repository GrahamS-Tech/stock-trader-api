namespace StockTraderAPI.Adapters
{
    public class holding
    {
        public virtual int Id { get; set; }
        public virtual profile? ProfileId { get; set; }
        public virtual string? Ticker { get; set;}
        public virtual int Shares { get; set; }
        public virtual DateTime LastTransactionDate { get; set; }

    }
}
