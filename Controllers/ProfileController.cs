using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTraderAPI.Adapters;
using System.Security.Claims;
using System.Text.Json;
using ISession = NHibernate.ISession;
namespace StockTraderAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class ProfileController : ControllerBase
{
    ISession session;

    public ProfileController(ISession session)
    {
        this.session = session;
    }

    [HttpGet("ProfileDetails")]
    [Authorize]
    public IActionResult GetProfileDetails()
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
        var currentProfile = new profile();
        currentProfile.ProfileId = profileData.ProfileId;
        currentProfile.FirstName = profileData.FirstName;
        currentProfile.LastName = profileData.LastName;
        currentProfile.EmailAddress = profileData.EmailAddress;
        currentProfile.Address1 = profileData.Address1;
        currentProfile.Address2 = profileData.Address2;
        currentProfile.City = profileData.City;
        currentProfile.State = profileData.State;
        currentProfile.PostalCode = profileData.PostalCode;
        currentProfile.ProfileIsActive = profileData.ProfileIsActive;
        currentProfile.ProfileComplete = profileData.ProfileComplete;

        response.Status = "success";
        response.Message = "Profile data retrieved successfully";
        response.Data = currentProfile;
        jsonResponse = JsonSerializer.Serialize(response);
        return Ok(jsonResponse);
    }

    [HttpPut("ProfileUpdate")]
    [Authorize]
    public IActionResult ProfileUpdate([FromBody] profile userProfile)
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
        Console.WriteLine(userProfile);
        var profileUpdate = new profile();
        var userId = int.Parse(currentUser.Id);
        var profileData = session.Query<profile>().FirstOrDefault(p => p.ProfileId == userId);
        profileUpdate.FirstName = userProfile.FirstName;
        profileUpdate.LastName = userProfile.LastName;
        profileUpdate.EmailAddress = userProfile.EmailAddress;
        profileUpdate.Address1 = userProfile.Address1;
        profileUpdate.Address2 = userProfile.Address2;
        profileUpdate.City = userProfile.City;
        profileUpdate.State = userProfile.State;
        profileUpdate.PostalCode = userProfile.PostalCode;
        profileUpdate.ProfileComplete = userProfile.ProfileComplete;
        session.SaveOrUpdate(profileUpdate);
        session.Flush();

        response.Status = "success";
        response.Message = "Profile data updated successfully";
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
