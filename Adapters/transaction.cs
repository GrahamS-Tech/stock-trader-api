namespace StockTraderAPI.Adapters;
    public class transaction
    {
        public virtual int Id { get; set; }
        public virtual profile? ProfileId { get; set; }
        public virtual string? Ticker { get; set; }
        public virtual double Shares { get; set; }
        public virtual double MarketValue { get; set; }
        public virtual string? TransactionType { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public virtual string? ShareName { get; set; }
    }