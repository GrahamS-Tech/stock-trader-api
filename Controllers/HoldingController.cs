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

    [HttpPost("AddHolding")]
    [Authorize]
    public IActionResult AddHolding([FromBody] holding newHolding)
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
        newHolding.ProfileId = profileData;
        newHolding.LastTransactionDate = DateTime.UtcNow;
        profileData.Holdings.Add(newHolding);
        session.Save(newHolding);
        session.Flush();

        response.Status = "success";
        response.Message = "Holding successfully added";
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    [HttpGet("GetAllHoldings")]
    [Authorize]
    public IActionResult GetAllHoldings()
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
        holdings = profileData.Holdings;
        List<holding> requestedHoldings = new List<holding>();

        foreach (var holding in holdings)
        {
            if (holding.Shares != 0)
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
        }

        Console.Write(holdings);
        response.Status = "success";
        response.Message = "All holdings successfully retrieved";
        response.Data = requestedHoldings;
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);

        //response.Status = "error";
        //response.Message = "No holdings found";
        //jsonResponse = JsonSerializer.Serialize(response);
        //return Ok(jsonResponse);

    }

    [HttpPut("UpdateHolding")]
    public IActionResult UpdateHolding([FromBody] holding updatedHolding)
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
        IList<holding> holdingDetails = new List<holding>();
        holdingDetails = profileData.Holdings;
        IEnumerable<holding> selectedHolding = holdingDetails.Where(i => i.Id == updatedHolding.Id);

        if (selectedHolding is null) {
            response.Status = "error";
            response.Message = "Holding not found";
            jsonResponse = JsonSerializer.Serialize(response);
            return NotFound(jsonResponse);
        }

        foreach (var holding in selectedHolding) {

            holding.Shares = updatedHolding.Shares;
            holding.LastTransactionDate = updatedHolding.LastTransactionDate;
            session.SaveOrUpdate(holding);
            session.Flush();
        }

        response.Status = "success";
        response.Message = "Holding updated successfully";
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