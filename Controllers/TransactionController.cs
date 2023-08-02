using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TransactionController
{
    ISession session;

    public TransactionController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("{profile_id}")]
    public string Get(int profile_id, int id)
    {

        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);

        IList<transaction> transactions = new List<transaction>();
        transactions = profileData.Transactions;
        List<transaction> requestedTransactionEntries = new List<transaction>();
        var jsonWatchlistEntries = "";

        string returnMessage = "";

        if (id == 0)
        {
            foreach (var transactionEntry in transactions)
            {
                transaction transaction = new transaction()
                {
                    Id = transactionEntry.Id,
                    Ticker = transactionEntry.Ticker,
                    Shares = transactionEntry.Shares,
                    TransactionType = transactionEntry.TransactionType,
                    TransactionDate = DateTime.UtcNow
                };

                requestedTransactionEntries.Add(transaction);
            }

            jsonWatchlistEntries = JsonSerializer.Serialize(requestedTransactionEntries);
            return jsonWatchlistEntries;
        }
        else
        {
            IEnumerable<transaction> selectedTransactionEntry = session.Query<transaction>().Where(i => i.Id == id);

            foreach (var transactionEntry in selectedTransactionEntry)
            {
                transaction transaction = new transaction()
                {
                    Id = transactionEntry.Id,
                    Ticker = transactionEntry.Ticker,
                    Shares = transactionEntry.Shares,
                    TransactionType = transactionEntry.TransactionType,
                    TransactionDate = DateTime.UtcNow
                };
                requestedTransactionEntries.Add(transaction);
            }

            jsonWatchlistEntries = JsonSerializer.Serialize(requestedTransactionEntries);
            return jsonWatchlistEntries;
        }
    }

    [HttpPost("{profile_id}")]
    public string Post(int profile_id, string ticker, double shares, string type)
    {
        profile profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);
        transaction newTransaction = new transaction();
        newTransaction.ProfileId = profileData;
        newTransaction.Ticker = ticker;
        newTransaction.Shares = shares;
        newTransaction.TransactionType = type;
        newTransaction.TransactionDate = DateTime.UtcNow;
        profileData.Transactions.Add(newTransaction);
        session.Save(newTransaction);
        session.Flush();

        return "New transaction successfully added";
    }
}
