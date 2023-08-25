using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class FiatHoldingController : ControllerBase
{
    ISession session;

    public FiatHoldingController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("GetAllFiatHoldings")]
    [Authorize]
    public IActionResult GetAllFiatHoldings()
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

        IList<fiat_holding> fiat_holdings = new List<fiat_holding>();
        fiat_holdings = profileData.FiatHoldings;
        List<fiat_holding> requestedFiatHoldings = new List<fiat_holding>();

        foreach (var fiat_holding in fiat_holdings)
        {
            if (fiat_holding.Balance != 0)
            {
                fiat_holding _fiat_holding = new fiat_holding()
                {
                    Id = fiat_holding.Id,
                    Currency = fiat_holding.Currency,
                    Balance = fiat_holding.Balance,
                    LastTransactionDate = fiat_holding.LastTransactionDate,
                };
                requestedFiatHoldings.Add(_fiat_holding);
            }
        }

        response.Status = "success";
        response.Message = "All fiat holdings successfully retrieved";
        response.Data = requestedFiatHoldings;
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