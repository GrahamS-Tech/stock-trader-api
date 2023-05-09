using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class HoldingController
{
    ISession session;

    public HoldingController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("{profile_id}")]
    public string Get(int profile_id, int id)
    {
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);

        IList<holding> holdings = new List<holding>();
        holdings = profileData.Holdings;
        List<holding> requestedHoldingEntries = new List<holding>();
        var jsonHoldingEntries = "";

        string returnMessage = "";

        if (id == 0)
        {
            foreach (var holdingEntry in holdings)
            {
                holding holding = new holding()
                {
                    Id = holdingEntry.Id,
                    Ticker = holdingEntry.Ticker,
                    Shares = holdingEntry.Shares,
                    LastTransactionDate = holdingEntry.LastTransactionDate
                };

                requestedHoldingEntries.Add(holding);
            }

            jsonHoldingEntries = JsonSerializer.Serialize(requestedHoldingEntries);
            return jsonHoldingEntries;
        }
        else
        {
            IEnumerable<holding> selectedHoldingEntry = session.Query<holding>().Where(i => i.Id == id);

            foreach (var holdingEntry in selectedHoldingEntry)
            {
                holding holding = new holding()
                {
                    Id = holdingEntry.Id,
                    Ticker = holdingEntry.Ticker,
                    Shares = holdingEntry.Shares,
                    LastTransactionDate = holdingEntry.LastTransactionDate
                };
                requestedHoldingEntries.Add(holding);
            }

            jsonHoldingEntries = JsonSerializer.Serialize(requestedHoldingEntries);
            return jsonHoldingEntries;
        }
    }

    [HttpPost("{profile_id}")]
    public string Post(int profile_id, string ticker, double shares)
    {
        profile profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);
        holding newHolding = new holding();
        newHolding.ProfileId = profileData;
        newHolding.Ticker = ticker;
        newHolding.Shares = shares;
        newHolding.LastTransactionDate = DateTime.Now;
        profileData.Holdings.Add(newHolding);
        session.Save(newHolding);
        session.Flush();

        return "New holding successfully added";
    }

    [HttpPut("{profile_id}, {id}")]
    public string Put(int profile_id, int id, double shares, DateTime lastTransactionDate)
    {
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);

        IList<holding> holdingDetails = new List<holding>();
        holdingDetails = profileData.Holdings;
        IEnumerable<holding> selectedHoldingDetails = holdingDetails.Where(i => i.Id == id);

        var returnMessage = "";

        foreach (var holdingDetail in selectedHoldingDetails)
        {
            holdingDetail.Shares = shares;
            holdingDetail.LastTransactionDate = lastTransactionDate;
            session.SaveOrUpdate(holdingDetail);
            session.Flush();

            returnMessage = "Holdings updated";
        }

        return returnMessage;
    }

}