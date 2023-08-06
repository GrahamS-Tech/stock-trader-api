namespace StockTraderAPI.Adapters
{
    public class watch_list
    {
        public virtual int Id { get; set; }
        public virtual profile? ProfileId { get; set; }
        public virtual string? Ticker { get; set; }
        public virtual string? Name { get; set; }
        public virtual DateTime DateAdded { get; set; }
        public virtual bool IsActive { get; set; }
    }
}
