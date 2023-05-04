namespace StockTraderAPI.Adapters
{
    public class banking_detail
    {
        public virtual int Id { get; set; }
        public virtual profile? ProfileId { get; set; }
        public virtual string? AccountType { get; set;}
        public virtual string? AccountNumber { get; set;}
        public virtual string? RoutingNumber { get; set;}
        public virtual DateTime DateAdded { get; set;}

    }
}
