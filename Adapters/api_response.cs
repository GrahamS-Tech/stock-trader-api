namespace StockTraderAPI.Adapters;

public class api_response<T>
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public T Data { get; set; }
}
