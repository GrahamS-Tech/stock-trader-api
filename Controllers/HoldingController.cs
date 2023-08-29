using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class HoldingController : ControllerBase
{
    ISession session;

    public HoldingController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("{GetAllHoldings}")]
    [Authorize]
    public IActionResult GetAllHoldings(bool excludeZero)
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

        IList<holding> holdings = new List<holding>();
        if (!excludeZero)
        {
            holdings = profileData.Holdings;
        }
        else {
            holdings = profileData.Holdings.Where(h => h.Shares != 0).ToList();
        }
        List<holding> requestedHoldings = new List<holding>();

        foreach (var holding in holdings)
        {
            holding _holding = new holding() 
                { 
                Id = holding.Id,
                Ticker = holding.Ticker,
                Name = holding.Name,
                Shares = holding.Shares,
                LastTransactionDate = holding.LastTransactionDate,
                };
                requestedHoldings.Add(_holding);
        }

        Console.Write(holdings);
        response.Status = "success";
        response.Message = "All holdings successfully retrieved";
        response.Data = requestedHoldings;
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