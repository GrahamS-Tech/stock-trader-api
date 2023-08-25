namespace StockTraderAPI.Adapters;

public class fiat_transaction
{
    public virtual int Id { get; set; }
    public virtual profile? ProfileId { get; set; }
    public virtual string? Currency { get; set;}
    public virtual double Value { get; set; }
    public virtual string? TransactionType { get; set; }
    public virtual string? AccountNumber { get; set; }
    public virtual DateTime TransactionDate { get; set; }

}
