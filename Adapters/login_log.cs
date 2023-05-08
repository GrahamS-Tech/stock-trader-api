namespace StockTraderAPI.Adapters
{
    public class login_log
    {
        public virtual int Id { get; set; }
        public virtual profile ProfileId { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual bool Successful { get; set; }

    }
}
