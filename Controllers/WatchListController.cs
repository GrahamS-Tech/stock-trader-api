using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class WatchListController
{
    ISession session;

    public WatchListController(ISession session)
    {
        this.session = session;
    }

    //Get by profile id returns all watchlist entries for user, including id returns a single entry
    [HttpGet("{profile_id}")]
    public string Get(int profile_id, int id)
    {
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);

        IList<watch_list> watchlistEntries = new List<watch_list>();
        watchlistEntries = profileData.WatchLists;
        List<watch_list> requestedWatchListEntries = new List<watch_list>();
        var jsonWatchlistEntries = "";

        string returnMessage = "";

        if (id == 0)
        {
            foreach (var watchListEntry in watchlistEntries)
            {
                watch_list watch_List = new watch_list()
                {
                    Id = watchListEntry.Id,
                    Ticker = watchListEntry.Ticker,
                    DateAdded = DateTime.UtcNow,
                    IsActive = watchListEntry.IsActive
                };

                requestedWatchListEntries.Add(watch_List);
            }

            jsonWatchlistEntries = JsonSerializer.Serialize(requestedWatchListEntries);
            return jsonWatchlistEntries;
        }
        else
        {
            IEnumerable<watch_list> selectedWatchlistEntry = session.Query<watch_list>().Where(i => i.Id == id);

            foreach (var watchListEntry in selectedWatchlistEntry)
            {
                watch_list watch_List = new watch_list()
                {
                Id = watchListEntry.Id,
                Ticker = watchListEntry.Ticker,
                DateAdded = DateTime.UtcNow,
                IsActive = watchListEntry.IsActive
                };
                requestedWatchListEntries.Add(watch_List);
            }

            jsonWatchlistEntries = JsonSerializer.Serialize(requestedWatchListEntries);
            return jsonWatchlistEntries;
        }
    }

    [HttpPost("{profile_id}")]
    public string Post(int profile_id, string ticker)
    {
        profile profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);
        watch_list newWatchListEntry = new watch_list();
        newWatchListEntry.ProfileId = profileData;
        newWatchListEntry.Ticker = ticker;
        newWatchListEntry.DateAdded = DateTime.UtcNow;
        newWatchListEntry.IsActive = true;
        profileData.WatchLists.Add(newWatchListEntry);
        session.Save(newWatchListEntry);
        session.Flush();

        return "New transaction successfully added";
    }

    [HttpPut("{profile_id}, {id}")]
    public string Put(int profile_id, int id)
    {

        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == profile_id);

        IList<watch_list> watchlistDetails = new List<watch_list>();
        watchlistDetails = profileData.WatchLists;
        IEnumerable<watch_list> selectedWatchlistDetails = watchlistDetails.Where(i => i.Id == id);

        var returnMessage = "";

        foreach (var watchlistDetail in selectedWatchlistDetails)
        {
            watchlistDetail.IsActive = false;
            session.SaveOrUpdate(watchlistDetails);
            session.Flush();

            returnMessage = "Watchlist entry removed";
        }

        return returnMessage;
    }

}
