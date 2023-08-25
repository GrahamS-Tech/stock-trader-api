namespace StockTraderAPI.Adapters;

public class fiat_holding
{
    public virtual int Id { get; set; }
    public virtual profile? ProfileId { get; set; }
    public virtual string? Currency { get; set; }
    public virtual double Balance { get; set; }
    public virtual DateTime LastTransactionDate { get; set; }

}
