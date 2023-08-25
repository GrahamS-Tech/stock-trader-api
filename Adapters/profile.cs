namespace StockTraderAPI.Adapters;

public class profile
{
    public virtual int ProfileId { get; set; }
    public virtual string? FirstName { get; set; }
    public virtual string? LastName { get; set; }
    public virtual string? EmailAddress { get; set; }
    public virtual string? Password { get; set; }
    public virtual DateTime SignupDate { get; set; }
    public virtual string? Address1 { get; set; }
    public virtual string? Address2 { get; set; }
    public virtual string? City { get; set; }
    public virtual string? State { get; set; }
    public virtual string? PostalCode { get; set; }
    public virtual bool ProfileIsActive { get; set; }
    public virtual bool ForcePasswordReset { get; set; }
    public virtual string? Salt { get; set; }
    public virtual bool ProfileComplete { get; set; }
    public virtual DateTime LastLoginTimestamp { get; set; }
    public virtual IList<banking_detail>? BankingDetails { get; set; }
    public virtual IList<holding>? Holdings { get; set; }
    public virtual IList<transaction>? Transactions { get; set; }
    public virtual IList<watch_list>? WatchLists { get; set; }
    public virtual IList<login_log>? LoginLog { get; set; }
    public virtual IList<fiat_holding>? FiatHoldings { get; set; }
    public virtual IList<fiat_transaction>? FiatTransactions { get; set; }
};