using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class TransactionController : ControllerBase
{
    ISession session;

    public TransactionController(ISession session)
    {
        this.session = session;
    }

    [HttpPost("AddTransaction")]
    [Authorize]
    public IActionResult AddTransaction([FromBody] transaction newTransaction)
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
        newTransaction.ProfileId = profileData;
        newTransaction.TransactionDate = DateTime.UtcNow;
        profileData.Transactions.Add(newTransaction);
        session.Save(newTransaction);
        session.Flush();

        response.Status = "success";
        response.Message = "Transaction successfully added";
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    [HttpGet("GetAllTransactions")]
    [Authorize]
    public IActionResult GetAllTransactions()
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
        IList<transaction> transactions = new List<transaction>();
        transactions = profileData.Transactions;

        if (transactions is null) {
            response.Status = "error";
            response.Message = "No transactions found";
            jsonResponse = JsonSerializer.Serialize(response);
            return Ok(jsonResponse);
        }

        response.Status = "success";
        response.Message = "Transactions successfully retrieved";
        response.Data = transactions;
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
