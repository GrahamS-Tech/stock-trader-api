using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using System.Text.Json;
using ISession = NHibernate.ISession;

namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class BankingDetailController : ControllerBase
{
    ISession session;

    public BankingDetailController(ISession session)
    {
        this.session = session;
    }

    [HttpPost("AddAccount")]
    [Authorize]
    public IActionResult AddBankAccount([FromBody] banking_detail bankAccount)
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
        banking_detail newBankingDetails = new banking_detail();
        newBankingDetails.ProfileId = profileData;
        newBankingDetails.AccountType = bankAccount.AccountType;
        newBankingDetails.AccountNumber = bankAccount.AccountNumber;
        newBankingDetails.RoutingNumber = bankAccount.RoutingNumber;
        newBankingDetails.DateAdded = DateTime.UtcNow;
        newBankingDetails.IsActive = true;
        profileData.BankingDetails.Add(newBankingDetails);
        session.Save(newBankingDetails);
        session.Flush();

        response.Status = "success";
        response.Message = "Account successfully added";
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    [HttpGet("GetAllAccounts")]
    [Authorize]
    public IActionResult GetAllAccounts()
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
        
        IList<banking_detail> bankingDetails = new List<banking_detail>();
        bankingDetails = profileData.BankingDetails;
        List<banking_detail> requestedBankingDetail = new List<banking_detail>();

            foreach (var bankingDetail in bankingDetails)
            {
                if (bankingDetail.IsActive == true)
                {                
                    banking_detail banking_Detail = new banking_detail()
                    {
                        Id = bankingDetail.Id,
                        AccountType = bankingDetail.AccountType,
                        AccountNumber = bankingDetail.AccountNumber,
                        RoutingNumber = bankingDetail.RoutingNumber,
                        DateAdded = bankingDetail.DateAdded,
                        IsActive = bankingDetail.IsActive
                    };

                    requestedBankingDetail.Add(banking_Detail);
                }
            }

        response.Status = "success";
        response.Message = "All accounts retrieved successfully";
        response.Data = requestedBankingDetail;
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    [HttpPut("DeactivateAccount")]
    [Authorize]
    public IActionResult DeactivateAccount([FromBody] int accountId)
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
        IList<banking_detail> bankingDetails = new List<banking_detail>();
        bankingDetails = profileData.BankingDetails;
        IEnumerable<banking_detail> selectedBankingDetail = bankingDetails.Where(i => i.Id == accountId);

        if (selectedBankingDetail is null)
        {
            response.Status = "error";
            response.Message = "Account not found";
            jsonResponse = JsonSerializer.Serialize(response);
            return NotFound(jsonResponse);
        }

        var returnMessage = "";

        foreach (var bankingDetail in selectedBankingDetail)
        {
            bankingDetail.IsActive = false;
            session.SaveOrUpdate(bankingDetail);
            session.Flush();
        }

        response.Status = "success";
        response.Message = "Account removed successfully";
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