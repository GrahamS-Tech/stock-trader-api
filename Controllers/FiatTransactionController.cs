using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class FiatTransactionController : ControllerBase
{
    ISession session;

    public FiatTransactionController(ISession session)
    {
        this.session = session;
    }

    [HttpPost("AddFiatTransaction")]
    [Authorize]
    public IActionResult AddFiatTransaction([FromBody] fiat_transaction newFiatTransaction)
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
        fiat_transaction new_fiat_transaction = new fiat_transaction();
        new_fiat_transaction.ProfileId = profileData;
        new_fiat_transaction.Currency = newFiatTransaction.Currency;
        new_fiat_transaction.Value = newFiatTransaction.Value;
        new_fiat_transaction.TransactionType = newFiatTransaction.TransactionType;
        new_fiat_transaction.AccountNumber = newFiatTransaction.AccountNumber;
        new_fiat_transaction.TransactionDate = DateTime.UtcNow;
        profileData.FiatTransactions.Add(new_fiat_transaction);
        session.Save(new_fiat_transaction);
        session.Flush();

        response.Status = "success";
        response.Message = "Fiat transaction successfully added";
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    [HttpGet("GetAllFiatTransactions")]
    [Authorize]
    public IActionResult GetAllFiatTransactions()
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
        IList<fiat_transaction> fiat_transactions = new List<fiat_transaction>();
        fiat_transactions = profileData.FiatTransactions;

        if (fiat_transactions is null)
        {
            response.Status = "error";
            response.Message = "No fiat transactions found";
            jsonResponse = JsonSerializer.Serialize(response);
            return Ok(jsonResponse);
        }

        response.Status = "success";
        response.Message = "Fiat transactions successfully retrieved";
        response.Data = fiat_transactions;
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
