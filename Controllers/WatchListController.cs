using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class WatchListController : ControllerBase
{
    ISession session;

    public WatchListController(ISession session)
    {
        this.session = session;
    }

    [HttpPost("AddWatchListEntry")]
    [Authorize]
    public IActionResult AddWatchListEntry([FromBody] watch_list watchlist)
    {
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null)
        {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonSerializer.Serialize(response);
            return NotFound(jsonResponse);
        }

        var userId = int.Parse(currentUser.Id);
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == userId);
        watch_list newWatchListEntry = new watch_list();
        newWatchListEntry.ProfileId = profileData;
        newWatchListEntry.Ticker = watchlist.Ticker;
        newWatchListEntry.Name = watchlist.Name;
        newWatchListEntry.DateAdded = DateTime.UtcNow;
        newWatchListEntry.IsActive = true;
        profileData.WatchLists.Add(newWatchListEntry);
        session.Save(newWatchListEntry);
        session.Flush();

        response.Status = "success";
        response.Message = "Watch list entry successfully added";
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    [HttpGet("GetWatchListEntries")]
    [Authorize]
    public IActionResult GetWatchListEntries()
    {
        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null)
        {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonSerializer.Serialize(response);
            return NotFound(jsonResponse);
        }

        var userId = int.Parse(currentUser.Id);
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == userId);

        IList<watch_list> watchlistEntries = new List<watch_list>();
        watchlistEntries = profileData.WatchLists;
        List<watch_list> requestedWatchListEntries = new List<watch_list>();

            foreach (var watchListEntry in watchlistEntries)
            {
                if (watchListEntry.IsActive) { 
                    watch_list watch_List = new watch_list()
                    {
                    Id = watchListEntry.Id,
                    Ticker = watchListEntry.Ticker,
                    Name = watchListEntry.Name,
                    DateAdded = watchListEntry.DateAdded,
                    IsActive = watchListEntry.IsActive
                    };
                    requestedWatchListEntries.Add(watch_List);
                }
            }

            response.Status = "success";
            response.Message = "All watch list entries retrieved successfully";
            response.Data = requestedWatchListEntries;
            jsonResponse = JsonSerializer.Serialize(response);
            return Ok(jsonResponse);
    }


    [HttpPut("DeactivateWatchListEntry")]
    [Authorize]
    public IActionResult DeactivateWatchListEntry([FromBody] int watchListId)
    {

        var response = new api_response<object> { };
        var jsonResponse = "";
        var currentUser = ValidateUser();

        if (currentUser.Id is null)
        {
            response.Status = "error";
            response.Message = "User account not found";
            jsonResponse = JsonSerializer.Serialize(response);
            return NotFound(jsonResponse);
        }

        var userId = int.Parse(currentUser.Id);
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == userId);
        IList<watch_list> watchlistDetails = new List<watch_list>();
        watchlistDetails = profileData.WatchLists;
        IEnumerable<watch_list> selectedWatchlistDetails = watchlistDetails.Where(i => i.Id == watchListId);

        if (selectedWatchlistDetails is null) {
            response.Status = "error";
            response.Message = "Watch list item not found";
            jsonResponse = JsonSerializer.Serialize(response);
            return NotFound(jsonResponse);
        }

        foreach (var watchlistDetail in selectedWatchlistDetails) {
            watchlistDetail.IsActive = false;
            session.SaveOrUpdate(watchlistDetail);
            session.Flush();
        }

        response.Status = "success";
        response.Message = "Watch list entry removed successfully";
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    private login ValidateUser()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        if (identity is not null)
        {
            var userClaims = identity.Claims;

            return new login
            {
                Id = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value
            };
        }
        return null;
    }

}
